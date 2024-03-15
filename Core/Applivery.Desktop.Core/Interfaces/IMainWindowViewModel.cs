using System.ComponentModel;
using System.Windows.Media;

namespace Applivery.Desktop.Core.Interfaces
{
    public interface IMainWindowViewModel : INotifyPropertyChanged
    {
        string Title { get; }
        ImageSource Icon { get; }

        string Id { get; set; }
        IViewModel ViewModel { get; set; }

        void SetSelectedPlugin();
    }
}
