// ****************************************************************************
// <copyright file="Messenger.cs" company="GalaSoft Laurent Bugnion">
// Copyright © GalaSoft Laurent Bugnion 2009-2016
// </copyright>
// ****************************************************************************
// <author>Laurent Bugnion</author>
// <email>laurent@galasoft.ch</email>
// <date>13.4.2009</date>
// <project>GalaSoft.MvvmLight.Messaging</project>
// <web>http://www.mvvmlight.net</web>
// <license>
// See license.txt in this project or http://www.galasoft.ch/license_MIT.txt
// </license>
// ****************************************************************************

using Applivery.Desktop.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Threading;

namespace Applivery.Desktop.Core.Events
{
    /// <summary>
    /// The Messenger is a class allowing objects to exchange messages.
    /// </summary>
    ////    [ClassInfo(typeof(Messenger),
    ////    VersionString = "5.4.21",
    ////    DateString = "201708281410",
    ////    Description = "A messenger class allowing a class to send a message to multiple recipients",
    ////    UrlContacts = "http://www.galasoft.ch/contact_en.html",
    ////    Email = "laurent@galasoft.ch")]
    [Export(typeof(IEventAggregator))]
    internal class EventAggregator : IEventAggregator
    {
        private static readonly object CreationLock = new object();
        private static IEventAggregator _defaultInstance;
        private readonly object _registerLock = new object();
        private Dictionary<Type, List<WeakActionAndToken>> _recipientsOfSubclassesAction;
        private Dictionary<Type, List<WeakActionAndToken>> _recipientsStrictAction;
        private bool _isCleanupRegistered;

        /// <summary>
        /// Gets the Messenger's default instance, allowing
        /// to register and send messages in a static manner.
        /// </summary>
        public static IEventAggregator Instance
        {
            get
            {
                if (_defaultInstance == null)
                {
                    lock (CreationLock)
                    {
                        if (_defaultInstance == null)
                        {
                            _defaultInstance = new EventAggregator();
                        }
                    }
                }

                return _defaultInstance;
            }
        }

        #region IMessenger Members

        /// <summary>
        /// Registers a recipient for a type of message TMessage. The action
        /// parameter will be executed when a corresponding message is sent.
        /// <para>Registering a recipient does not create a hard reference to it,
        /// so if this recipient is deleted, no memory leak is caused.</para>
        /// </summary>
        /// <typeparam name="TMessage">The type of message that the recipient registers
        /// for.</typeparam>
        /// <param name="recipient">The recipient that will receive the messages.</param>
        /// <param name="action">The action that will be executed when a message
        /// of type TMessage is sent. IMPORTANT: If the action causes a closure,
        /// you must set keepTargetAlive to true to avoid side effects. </param>
        /// <param name="keepTargetAlive">If true, the target of the Action will
        /// be kept as a hard reference, which might cause a memory leak. You should only set this
        /// parameter to true if the action is using closures. See
        /// http://galasoft.ch/s/mvvmweakaction. </param>
        public virtual void Register<TMessage>(object recipient, Action<TMessage> action, bool keepTargetAlive = false, object token = null)
        {
            lock (_registerLock)
            {
                var messageType = typeof(TMessage);

                Dictionary<Type, List<WeakActionAndToken>> recipients;

                if (_recipientsStrictAction == null)
                {
                    _recipientsStrictAction = new Dictionary<Type, List<WeakActionAndToken>>();
                }

                recipients = _recipientsStrictAction;

                lock (recipients)
                {
                    List<WeakActionAndToken> list;

                    if (!recipients.ContainsKey(messageType))
                    {
                        list = new List<WeakActionAndToken>();
                        recipients.Add(messageType, list);
                    }
                    else
                    {
                        list = recipients[messageType];
                    }

                    var weakAction = new WeakAction<TMessage>(recipient, action, keepTargetAlive);

                    var item = new WeakActionAndToken
                    {
                        Action = weakAction,
                        Token = token
                    };

                    list.Add(item);
                }
            }

            RequestCleanup();
        }

        /// <summary>
        /// Sends a message to registered recipients. The message will
        /// reach all recipients that registered for this message type
        /// using one of the Register methods.
        /// </summary>
        /// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
        /// <param name="message">The message to send to registered recipients.</param>
        public virtual void Send<TMessage>(TMessage message)
        {
            SendToTargetOrType(message, null, null);
        }

        /// <summary>
        /// Unregisters a messager recipient completely. After this method
        /// is executed, the recipient will not receive any messages anymore.
        /// </summary>
        /// <param name="recipient">The recipient that must be unregistered.</param>
        public virtual void Unregister(object recipient)
        {
            UnregisterFromLists(recipient, _recipientsOfSubclassesAction);
            UnregisterFromLists(recipient, _recipientsStrictAction);
            RequestCleanup();
        }

        #endregion

        /// <summary>
        /// Provides a way to override the Messenger.Default instance with
        /// a custom instance, for example for unit testing purposes.
        /// </summary>
        /// <param name="newMessenger">The instance that will be used as Messenger.Default.</param>
        public static void OverrideDefault(IEventAggregator newMessenger)
        {
            _defaultInstance = newMessenger;
        }

        private static void CleanupList(IDictionary<Type, List<WeakActionAndToken>> lists)
        {
            if (lists == null)
            {
                return;
            }

            lock (lists)
            {
                var listsToRemove = new List<Type>();
                foreach (var list in lists)
                {
                    var recipientsToRemove = list.Value
                        .Where(item => item.Action == null || !item.Action.IsAlive)
                        .ToList();

                    foreach (var recipient in recipientsToRemove)
                    {
                        list.Value.Remove(recipient);
                    }

                    if (list.Value.Count == 0)
                    {
                        listsToRemove.Add(list.Key);
                    }
                }

                foreach (var key in listsToRemove)
                {
                    lists.Remove(key);
                }
            }
        }

        private static void SendToList<TMessage>(TMessage message, IEnumerable<WeakActionAndToken> weakActionsAndTokens, Type messageTargetType, object token)
        {
            if (weakActionsAndTokens != null)
            {
                // Clone to protect from people registering in a "receive message" method
                // Correction Messaging BL0004.007
                var list = weakActionsAndTokens.ToList();
                var listClone = list.Take(list.Count).ToList();

                foreach (var item in listClone)
                {
                    var executeAction = item.Action as IExecuteWithObject;

                    if (executeAction != null
                        && item.Action.IsAlive
                        && item.Action.Target != null
                        && (messageTargetType == null
                            || item.Action.Target.GetType() == messageTargetType
                            || messageTargetType.IsAssignableFrom(item.Action.Target.GetType()))
                        && ((item.Token == null && token == null)
                            || item.Token != null && item.Token.Equals(token)))
                    {
                        executeAction.ExecuteWithObject(message);
                    }
                }
            }
        }

        private static void UnregisterFromLists(object recipient, Dictionary<Type, List<WeakActionAndToken>> lists)
        {
            if (recipient == null
                || lists == null
                || lists.Count == 0)
            {
                return;
            }

            lock (lists)
            {
                foreach (var messageType in lists.Keys)
                {
                    foreach (var item in lists[messageType])
                    {
                        var weakAction = (IExecuteWithObject)item.Action;

                        if (weakAction != null
                            && recipient == weakAction.Target)
                        {
                            weakAction.MarkForDeletion();
                        }
                    }
                }
            }
        }

        private static void UnregisterFromLists<TMessage>(object recipient, object token, Action<TMessage> action, Dictionary<Type, List<WeakActionAndToken>> lists)
        {
            var messageType = typeof(TMessage);

            if (recipient == null
                || lists == null
                || lists.Count == 0
                || !lists.ContainsKey(messageType))
            {
                return;
            }

            lock (lists)
            {
                foreach (var item in lists[messageType])
                {
                    var weakActionCasted = item.Action as WeakAction<TMessage>;

                    if (weakActionCasted != null
                        && recipient == weakActionCasted.Target
                        && (action == null
                            || action.Method.Name == weakActionCasted.MethodName)
                        && (token == null
                            || token.Equals(item.Token)))
                    {
                        item.Action.MarkForDeletion();
                    }
                }
            }
        }

        /// <summary>
        /// Notifies the Messenger that the lists of recipients should
        /// be scanned and cleaned up.
        /// Since recipients are stored as <see cref="WeakReference"/>, 
        /// recipients can be garbage collected even though the Messenger keeps 
        /// them in a list. During the cleanup operation, all "dead"
        /// recipients are removed from the lists. Since this operation
        /// can take a moment, it is only executed when the application is
        /// idle. For this reason, a user of the Messenger class should use
        /// <see cref="RequestCleanup"/> instead of forcing one with the 
        /// <see cref="Cleanup" /> method.
        /// </summary>
        public void RequestCleanup()
        {
            if (!_isCleanupRegistered)
            {
                Action cleanupAction = Cleanup;

                Dispatcher.CurrentDispatcher.BeginInvoke(
                    cleanupAction,
                    DispatcherPriority.ApplicationIdle,
                    null);
                _isCleanupRegistered = true;
            }
        }

        /// <summary>
        /// Scans the recipients' lists for "dead" instances and removes them.
        /// Since recipients are stored as <see cref="WeakReference"/>, 
        /// recipients can be garbage collected even though the Messenger keeps 
        /// them in a list. During the cleanup operation, all "dead"
        /// recipients are removed from the lists. Since this operation
        /// can take a moment, it is only executed when the application is
        /// idle. For this reason, a user of the Messenger class should use
        /// <see cref="RequestCleanup"/> instead of forcing one with the 
        /// <see cref="Cleanup" /> method.
        /// </summary>
        public void Cleanup()
        {
            CleanupList(_recipientsOfSubclassesAction);
            CleanupList(_recipientsStrictAction);
            _isCleanupRegistered = false;
        }

        private void SendToTargetOrType<TMessage>(TMessage message, Type messageTargetType, object token)
        {
            var messageType = typeof(TMessage);

            if (_recipientsOfSubclassesAction != null)
            {
                // Clone to protect from people registering in a "receive message" method
                // Correction Messaging BL0008.002
                var listClone =
                    _recipientsOfSubclassesAction.Keys.Take(_recipientsOfSubclassesAction.Count()).ToList();

                foreach (var type in listClone)
                {
                    List<WeakActionAndToken> list = null;

                    if (messageType == type
                        || messageType.IsSubclassOf(type)
                        || type.IsAssignableFrom(messageType))
                    {
                        lock (_recipientsOfSubclassesAction)
                        {
                            list = _recipientsOfSubclassesAction[type].Take(_recipientsOfSubclassesAction[type].Count).ToList();
                        }
                    }

                    SendToList(message, list, messageTargetType, token);
                }
            }

            if (_recipientsStrictAction != null)
            {
                List<WeakActionAndToken> list = null;

                lock (_recipientsStrictAction)
                {
                    if (_recipientsStrictAction.ContainsKey(messageType))
                    {
                        list = _recipientsStrictAction[messageType]
                            .Take(_recipientsStrictAction[messageType].Count())
                            .ToList();
                    }
                }

                if (list != null)
                {
                    SendToList(message, list, messageTargetType, token);
                }
            }

            RequestCleanup();
        }

        #region Nested type: WeakActionAndToken

        private struct WeakActionAndToken
        {
            public WeakAction Action;
            public object Token;
        }
        #endregion
    }
}
