﻿using PropertyChanged;
using System.Collections.ObjectModel;

namespace Applivery.Desktop.Comics.Models
{
    /// <summary>
    /// Comics data
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class DisplayerModel
    {
        public DisplayerModel() { }

        public ObservableCollection<CreatorComicModel> CreatorComics { get; set; } = new ObservableCollection<CreatorComicModel>();
        public CreatorComicModel SelectedCreator { get; set; }
    }
}