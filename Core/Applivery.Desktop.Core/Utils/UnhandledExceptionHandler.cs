using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace Applivery.Desktop.Core.Utils
{
    /// <summary>
    /// Catch exception unhandled avoiding the crash.
    /// reference: https://learn.microsoft.com/en-us/dotnet/api/system.appdomain.unhandledexception?view=net-8.0
    /// </summary>
    public class UnhandledExceptionHandler
    {
        private static readonly string unhandledExceptionLog = "Exception in Applivery.MarvelComics.Desktop";

        /// <summary>
        /// Constructor
        /// </summary>
        static UnhandledExceptionHandler()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(AppDomainUnhandledException);
            if (Application.Current != null)
                Application.Current.DispatcherUnhandledException += ApplicationDispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += UnobservedTaskException;
        }

        /// <summary>
        /// Help to init the static method in bootstrapper.
        /// </summary>
        public static void Init() { }

        #region Handlers

        /// <summary>
        /// Occurs when an exception is not caught. Catch the Unhandled exception in the CurrentDomain.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e != null && e.IsTerminating)
            {
                LogException(e.ExceptionObject as Exception, !e.IsTerminating);
                MessageBox.Show((e.ExceptionObject as Exception).ToString(), unhandledExceptionLog, MessageBoxButton.OK, MessageBoxImage.Stop);
                Application.Current.Shutdown();
            }
            else
            {
                LogException(e != null ? e.ExceptionObject as Exception : null, true);
            }
        }

        /// <summary>
        /// Occurs when an exception is thrown by an application but not handled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void ApplicationDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            LogException(e.Exception, true);
        }

        /// <summary>
        /// Occurs when a faulted task's unobserved exception is about to trigger exception escalation policy, which, by default, would terminate the process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void UnobservedTaskException(object sender, System.Threading.Tasks.UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            LogException(e.Exception, true);
        }

        #endregion Handlers

        #region Logs

        /// <summary>
        /// Log the exception and show a MessageBox if showTerminationDialog is true.
        /// The param showTerminationDialog can be used with the instruction #if DEBUG to avoid show the MessageBox in Release
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="showTerminationDialog"></param>
        private static bool LogException(Exception ex, bool showTerminationDialog)
        {
            try
            {
                string innerExceptionMessage = string.Empty;

                // Get the Inner Exception as message.
                if (ex != null)
                {
                    Exception temp = ex;
                    innerExceptionMessage = "---------- Inner Exception ---------- \r\n\t" + ex.Message + "\r\n";
                    while (temp.InnerException != null)
                    {
                        innerExceptionMessage += "\t" + temp.InnerException.Message + "\r\n";
                        temp = temp.InnerException;
                    }

                    innerExceptionMessage += "\r\nSource: " + ex.Source + "\r\n";

                    // TODO FRAN: no hace falta esta ultima
                    innerExceptionMessage += "Full exception: " + temp.ToString();

                    Trace.TraceError($"\r\n---------------  {unhandledExceptionLog}  --------------\r\n {innerExceptionMessage}\r\n");
                }
                else
                {
                    Trace.TraceError($"\r\n------------- {unhandledExceptionLog} -------------\r\n");
                }

                if (showTerminationDialog)
                {
                    ShowExceptionMessageBox(unhandledExceptionLog, innerExceptionMessage);
                }

                return true;
            }
            catch (Exception ex2)
            {
                try
                {
                    MessageBox.Show("Fatal Non-UI Error:\r\n" + ex2.Message, "Fatal Non-UI Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return false;
                }
                finally
                {
                    Application.Current.Shutdown();
                }
            }
        }

        private static void ShowExceptionMessageBox(string title, string innerExceptionMessage)
        {
            string message = "Check the logs for more info.\r\n";

#if DEBUG
            if (!string.IsNullOrEmpty(innerExceptionMessage))
            {
                message += innerExceptionMessage;
            }
            else
            {
                message += "Unknown exception.";
            }
#endif
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Stop);
        }

        #endregion Logs
    }
}
