using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;

namespace Applivery.Desktop.Core.Interfaces
{
    /// <summary>
    /// Interface IPluginItem
    /// </summary>
    public interface IPluginItem : INotifyPropertyChanged
    {
        string Id { get; set; }

        string Name { get; set; }

        ICommand Command { get; set; }
    }
}
