using Applivery.Desktop.Core.Interfaces;
using System.ComponentModel;

namespace Applivery.Desktop.Core.Base
{
    /// <summary>
    /// The content of the user controls will be set in this viewmodel
    /// </summary>
    public sealed class GenericViewModel : IWindowViewModel
    {
        /// <summary>
        /// The view
        /// </summary>
        private System.Windows.Controls.Page _view;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericViewModel"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        public GenericViewModel(IViewModel content)
        {
            _view = GetView();
            Initialize(content);
        }

        #endregion

        #region IWindow events

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IWindow Properties

        /// <summary>
        /// Gets or sets the content of the page.
        /// </summary>
        /// <value>
        /// The content of the page.
        /// </value>
        public IViewModel ViewModel { get; set; }

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        public System.Windows.Controls.Page View { get { return _view; } }

        #endregion

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <returns></returns>
        private System.Windows.Controls.Page GetView()
        {
            if (_view == null)
                return new GenericView();
            else
                return _view;
        }

        /// <summary>
        /// Initializes the specified window content.
        /// </summary>
        /// <param name="windowContent">Content of the window.</param>
        private void Initialize(IViewModel windowContent)
        {
            ViewModel = windowContent;
            _view.DataContext = this;
        }
    }
}