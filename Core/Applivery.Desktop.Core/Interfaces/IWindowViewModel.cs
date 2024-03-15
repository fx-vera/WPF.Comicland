using System.ComponentModel;

namespace Applivery.Desktop.Core.Interfaces
{
    public interface IWindowViewModel : INotifyPropertyChanged
    {
        IViewModel ViewModel { get; set; }

        System.Windows.Controls.Page View { get; }
    }
}