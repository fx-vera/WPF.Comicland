using Applivery.Desktop.Core.Base;
using Applivery.Desktop.Core.Interfaces;
using System.ComponentModel.Composition;

namespace Applivery.Desktop.Core.Managers
{
    [Export(typeof(IPageManager))]
    public class PageManager : IPageManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageManager"/> class.
        /// </summary>
        [ImportingConstructor]
        public PageManager()
        {
        }

        #region IPageManager functions

        public IWindowViewModel SetPageInMainWindow(IViewModel newPage)
        {
            if (newPage == null)
            {
                return null;
            }

            var mainframe = IoC.Get<IMainWindowViewModel>();
            var openedInstance = CreateNewWindow(newPage);
            mainframe.ViewModel = openedInstance.ViewModel;
            return openedInstance;
        }

        #endregion

        private IWindowViewModel CreateNewWindow(IViewModel newPage)
        {
            IWindowViewModel newWindowViewModel = new GenericViewModel(newPage);
            if (newWindowViewModel != null)
            {
                newWindowViewModel.View.DataContext = newWindowViewModel;
            }
            return newWindowViewModel;
        }
    }
}