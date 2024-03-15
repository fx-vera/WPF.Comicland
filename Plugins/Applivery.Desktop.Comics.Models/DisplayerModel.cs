using PropertyChanged;
using System.Collections.ObjectModel;

namespace Applivery.Desktop.Comics.Models
{
    /// <summary>
    /// Comic data
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class DisplayerModel
    {
        public DisplayerModel() { }

        public ObservableCollection<ComicModel> Comics { get; set; } = new ObservableCollection<ComicModel>();
    }
}
