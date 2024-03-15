using Applivery.Desktop.Core.Base;
using System.Collections.Generic;

namespace Applivery.Desktop.Launcher
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Bootstrapper
    {
        public override void InitPluginNames()
        {
            DllAllowed = new List<string>
            {
                "Applivery.Desktop"
            };
        }
    }
}
