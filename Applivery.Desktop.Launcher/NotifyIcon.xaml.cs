using System;
using System.Windows;

namespace Applivery.Desktop.Launcher
{
    /// <summary>
    /// Lógica de interacción para NotifyIcon.xaml
    /// </summary>
    public partial class NotifyIcon : Application
    {
        App app = null;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Icon = new System.Drawing.Icon("..\\..\\Applivery.Desktop.Launcher\\ca.ico"),
                Text = "Applivery.Desktop.Launcher",
                Visible = true
            };

            notifyIcon.Click += (sender, ex) =>
            {
                // Show a context menu or perform an action

                if (app == null)
                {
                    app = new App();
                }
                app.Show();
            };

            notifyIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("Close", null, this.MenuTest1_Click);
        }

        void MenuTest1_Click(object sender, EventArgs e)
        {
            app.Close();
            //Current.Shutdown();
            //Application.Exit();
        }
    }
}