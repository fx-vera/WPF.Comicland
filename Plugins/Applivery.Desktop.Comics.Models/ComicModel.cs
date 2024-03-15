using PropertyChanged;

namespace Applivery.Desktop.Comics.Models
{
    /// <summary>
    /// Model to store the Name and description of the comic
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class ComicModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
