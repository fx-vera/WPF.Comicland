using Applivery.Desktop.Core.Interfaces;
using PropertyChanged;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;

namespace Applivery.Desktop.Core.MVVM
{
    [AddINotifyPropertyChangedInterface]
    public class PluginItemBase : IPluginItem
    {
        public PluginItemBase()
        {
        }

        public PluginItemBase(string id, string name, ICommand command)
        {
            Id = id;
            Name = name;
            Command = command;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public ICommand Command { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
