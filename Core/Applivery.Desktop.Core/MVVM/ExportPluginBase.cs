using Applivery.Desktop.Core.Interfaces;
using Applivery.Desktop.Core.Managers;
using Applivery.Desktop.Core.Utils;
using System;

namespace Applivery.Desktop.Core.MVVM
{
    /// <summary>
    /// Class to Export the Plugin and make avaiable to IoC reading.
    /// </summary>
    public abstract class ExportPluginBase
    {
        protected ExportPluginBase()
        {
            LoadPlugins();
        }

        public abstract void LoadPlugins();

        public RelayCommand MenuItemCommand(PluginItemBase menuItem, Func<ViewModel> pageCreator)
        {
            return new RelayCommand(param => OnCommand(menuItem, pageCreator()), null);
        }

        /// <summary>
        /// Assign this command to your MenuItemBase.Command
        /// </summary>
        /// <param name="menuItem"></param>
        /// <param name="page"></param>
        /// <param name="showInMainframe"></param>
        private void OnCommand(PluginItemBase menuItem, ViewModel page)
        {
            page.Id = menuItem.Id;
            var pm = IoC.Get<IPageManager>();
            pm.SetPageInMainWindow(page);
        }
    }
}