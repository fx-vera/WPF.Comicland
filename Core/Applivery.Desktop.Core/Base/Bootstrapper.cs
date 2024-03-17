using Applivery.Desktop.Core.Events;
using Applivery.Desktop.Core.Interfaces;
using Applivery.Desktop.Core.Managers;
using Applivery.Desktop.Core.Models;
using Applivery.Desktop.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Applivery.Desktop.Core.Base
{
    /// <summary>
    /// Base class to manage the basic application operations and the main window.
    /// This class can be extended depending on requirements
    /// </summary>
    public abstract class Bootstrapper : Application
    {
        /// <summary>
        /// Contructor called by Application.
        /// </summary>
        public Bootstrapper()
        {
            UnhandledExceptionHandler.Init();
        }

        /// <summary>
        /// List of substrings of the dll names that WILL be allowed when trying MEF composition
        /// </summary>
        public List<string> DllAllowed { get; set; }

        /// <summary>
        /// Several steps: parse the info, creates the mainframe and the mission control bar
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            //Load application modules
            List<Assembly> assembliesToCompose = GetAssembliesForIoC();

            assembliesToCompose.Add(GetType().Assembly);

            Dictionary<Type, object> knownInstances = new Dictionary<Type, object>
            {
                { GetType(), this }
            };

            var eventAggregator = EventAggregator.Instance;
            knownInstances.Add(typeof(IEventAggregator), eventAggregator);

            string loadError = string.Empty;
            bool modulesLoadedOk = false;

            try
            {
                MEFIoCManager.Instance.LoadModules(knownInstances, assembliesToCompose);
                modulesLoadedOk = true;
            }
            catch (Exception ex)
            {
                loadError = ex.Message;
            }

            if (!modulesLoadedOk)
            {
                MessageBox.Show(loadError, "Error Loading Application Modules in Applivery.MarvelComics.Desktop.");
                Current.Shutdown();
                return;
            }

            LoadApplicationViews();
        }

        #region Abstracts

        /// <summary>
        /// Used to load by IoC the specific dlls, not all in the workspace.
        /// </summary>
        public abstract void InitPluginNames();

        #endregion Abstract

        #region Private

        /// <summary>
        /// Retrieves the list of assemblies containing parts or modules 
        /// that must be composed using MEF and instantiated through IoC.
        /// In this case, it only adds the current executing assembly. 
        /// If a subclass overrides this, it should call this base implementation, 
        /// take the returned list and then add whatever it wants to it, and return it.
        /// </summary>
        /// <returns>The list of assemblies to be loaded and composed</returns>
        private List<Assembly> GetAssembliesForIoC()
        {
            InitPluginNames();
            List<Assembly> possiblePlugins = new List<Assembly>();
            string exeDir = Path.GetDirectoryName(GetType().Assembly.Location);

            string[] dllsInDir = Directory.GetFiles(exeDir, "*.dll");

            for (int i = 0; i < dllsInDir.Length; i++)
            {
                string dllWithPath = dllsInDir[i];
                string dllName = Path.GetFileName(dllWithPath);

                bool tryLoadDll = DllAllowed.Any(x => dllName.IndexOf(x, StringComparison.InvariantCultureIgnoreCase) >= 0);

                if (tryLoadDll)
                {
                    possiblePlugins.Add(Assembly.LoadFile(dllWithPath));
                }
            }

            return possiblePlugins;
        }

        private void LoadApplicationViews()
        {
            IoC.Get<ViewsManager>().LoadAvailableViews();
            MEFIoCManager.Instance.LoadModules(new Dictionary<Type, object>(), GetAssembliesForIoC());
        }

        /// <summary>
        /// Function called to create the main application window with
        /// any implementation the subclasses want.
        /// </summary>
        /// <returns></returns>
        protected Window CreateMainWindow()
        {
            IMainWindowViewModel mainViewModel = IoC.Get<IMainWindowViewModel>();
            if (mainViewModel == null)
            {
                MessageBox.Show("Could not load main window component. Closing down.", "Error");
                return null;
            }
            MainWindow mainWindow = new MainWindow(mainViewModel);

            mainWindow.Dispatcher.BeginInvoke(new Action(() => mainWindow.SetCurrentValue(Window.TopmostProperty, false)), System.Windows.Threading.DispatcherPriority.ApplicationIdle, null);

            return mainWindow;
        }

        protected void CustomClosing()
        {
            //MainWindow.Close();
            //Current.Shutdown();
        }

        protected void LoadPlugins()
        {
            IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();
            LoadPluginEventArgs args;
            var pluginItems = IoC.GetAll<IPluginItem>();

            foreach (var item in pluginItems)
            {
                args = new LoadPluginEventArgs
                {
                    Id = item.Id,
                    Item = item,
                    IsCommand = true
                };
                eventAggregator.Send(args);
            }
        }

        #endregion Private
    }
}