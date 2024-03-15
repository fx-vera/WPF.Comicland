using System.Windows;
using System.Windows.Controls;

namespace Applivery.Desktop.Core.Base
{
    /// <summary>
    /// This is the parent of the user controls.
    /// </summary>
    public sealed partial class GenericView : Page
    {
        private GenericViewModel _viewModel;
        public GenericView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = this.DataContext as GenericViewModel;
        }
    }
}