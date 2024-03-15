using Applivery.Desktop.Core.Interfaces;
using PropertyChanged;

namespace Applivery.Desktop.Core.MVVM
{
    /// <summary>
    /// ViewModel
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public abstract class ViewModel : IViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        public ViewModel()
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