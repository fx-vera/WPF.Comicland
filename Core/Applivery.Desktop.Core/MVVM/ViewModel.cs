using Applivery.Desktop.Core.Interfaces;
using PropertyChanged;

namespace Applivery.Desktop.Core.MVVM
{
    /// <summary>
    /// ViewModelBase
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public abstract class ViewModelBase : IViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// </summary>
        public ViewModelBase()
        {

        }

        #region Properties

        /// <summary>
        /// Id to distinguish this page, for common events and saving the layout
        /// </summary>
        public string Id { get; set; }

        #endregion Properties
    }
}