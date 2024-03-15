using Applivery.Desktop.Core.Interfaces;
using System.Windows;

namespace Applivery.Desktop.Core.Base
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.ShowInTaskbar = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public MainWindow(IMainWindowViewModel viewModel) : this()
        {
            this.DataContext = viewModel;
        }
    }
}
