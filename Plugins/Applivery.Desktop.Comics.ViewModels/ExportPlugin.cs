using Applivery.Desktop.Comics.Views;
using Applivery.Desktop.Core.Interfaces;
using Applivery.Desktop.Core.MVVM;
using System;
using System.ComponentModel.Composition;

namespace Applivery.Desktop.Comics.ViewModels
{
    /// <summary>
    /// Allow to set the plugin in the main window by IoC.
    /// </summary>
    public class ExportPlugin : ExportPluginBase
    {
        public override void LoadPlugins()
        {
            ComicsDisplayer = new PluginItemBase();
            ComicsDisplayer.Id = "{4D36ABEE-72EA-4B3C-AA37-22A3D0CA613D}";
            ComicsDisplayer.Name = "Marvel Comics";
            ComicsDisplayer.Icon = null;
            ComicsDisplayer.Command = this.MenuItemCommand(ComicsDisplayer, new Func<ViewModel>(() => { return new DisplayerViewModel(); }));
        }

        [Export(typeof(IPluginItem))]
        public PluginItemBase ComicsDisplayer { get; set; }
    }

    [Export(typeof(IVVMMappingBase))]
    public class ComicsDisplayerViewMapping : ViewViewModelMappingBase<DisplayerViewModel, DisplayerView> { }
}
