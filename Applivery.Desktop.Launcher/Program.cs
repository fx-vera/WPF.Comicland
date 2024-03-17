using System;

namespace Applivery.Desktop.Launcher
{
    class Program
    {
        static App app = null;
        static void Main(string[] args)
        {
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
                app.Run();
            };

            notifyIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("Close", null, MenuTest1_Click);
        }
        static void MenuTest1_Click(object sender, EventArgs e)
        {
            app.Shutdown();
            //Current.Shutdown();
            //Application.Exit();
        }
    }
}
