using Applivery.Desktop.Core.Base;
using Applivery.Desktop.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Applivery.Desktop.Launcher
{
    /// <summary>
    /// Lógica de interacción para NotifyIcon.xaml
    /// </summary>
    public partial class App : Bootstrapper
    {
        System.Windows.Forms.NotifyIcon notifyIcon;

        public override void InitPluginNames()
        {
            DllAllowed = new List<string>
            {
                "Applivery.Desktop"
            };
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Icon = Launcher.Properties.Resources.ca,
                Text = "Applivery.Desktop.Launcher",
                Visible = true
            };

            notifyIcon.DoubleClick += (sender, ex) =>
            {
                Open_Click(null, null);
            };

            notifyIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("Open", null, this.Open_Click);
            notifyIcon.ContextMenuStrip.Items.Add("Close", null, this.Close_Click);
        }

        void Open_Click(object sender, EventArgs e)
        {
            MainWindow = CreateMainWindow();
            if (MainWindow == null)
            {
                MessageBox.Show("Error", "Error creating the MainWindow for the Application Modules in Applivery.MarvelComics.Desktop.");
            }
            else
            {
                MainWindow.Closing += (o, ee) => CustomClosing();
                LoadPlugins();
                ((IMainWindowViewModel)MainWindow.DataContext).SetSelectedPlugin();
                MainWindow.Show();
            }
        }

        void Close_Click(object sender, EventArgs e)
        {
            Current.Shutdown();
        }
    }
}