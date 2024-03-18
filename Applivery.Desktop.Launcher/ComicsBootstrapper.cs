using Applivery.Desktop.Core.MVVM;
using System.Collections.Generic;

namespace Applivery.Desktop.Launcher
{
    public class ComicsBootstrapper : BootstrapperBase
    {
        public override List<string> InitDllAllowed()
        {
            return new List<string>
            {
                "Applivery.Desktop"
            };
        }

        protected override System.Drawing.Icon SetNotifyIcon()
        {
            return Launcher.Properties.Resources.ca;
        }
    }
}