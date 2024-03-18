using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Applivery.Desktop.Comics.Models
{
    [AddINotifyPropertyChangedInterface]
    public class CreatorComicModel
    {
        public string Name { get; set; }
        private ObservableCollection<Models.ComicJSON.Result> comics = new ObservableCollection<ComicJSON.Result> { };
        public ObservableCollection<Models.ComicJSON.Result> Comics

        {
            get
            {
                return comics;
            }
            set
            {
                comics = value;
                SelectedComic = comics?.FirstOrDefault();
            }
        }
        private Models.ComicJSON.Result selectedComic;
        public Models.ComicJSON.Result SelectedComic
        {
            get { return selectedComic; }
            set
            {
                selectedComic = value;
                GetImageAsync();
            }
        }

        public string Description
        {
            get
            {
                if (SelectedComic == null) return string.Empty;
                if (SelectedComic.textObjects.Length == 0) return SelectedComic.title;

                string description = string.Empty;
                foreach (var item in SelectedComic.textObjects)
                {
                    description = description + item.text + "\r\n";
                }
                return description.Replace("<br>", string.Empty);
            }
        }

        public string Characters
        {
            get
            {
                if (SelectedComic == null) return string.Empty;
                if (SelectedComic.characters.items.Length == 0) return string.Empty;

                string summary = string.Empty;
                foreach (var item in SelectedComic.characters.items)
                {
                    summary = summary + item.name + "\r\n";
                }
                return summary.Replace("<br>", string.Empty);
            }
        }

        public string Stories
        {
            get
            {
                if (SelectedComic == null) return string.Empty;
                if (SelectedComic.stories.items.Length == 0) return string.Empty;

                string summary = string.Empty;
                foreach (var item in SelectedComic.stories.items)
                {
                    summary = summary + item.name + "\r\n";
                }
                return summary.Replace("<br>", string.Empty);
            }
        }

        public string CoCreators
        {
            get
            {
                if (SelectedComic == null) return string.Empty;
                if (SelectedComic.creators.items.Length < 1) return string.Empty;

                string summary = string.Empty;
                foreach (var item in SelectedComic.creators.items)
                {
                    summary = summary + item.name + $" ({item.role})" + "\r\n";
                }
                return summary.Replace("<br>", string.Empty);
            }
        }

        public string Series
        {
            get
            {
                if (SelectedComic == null) return string.Empty;

                return SelectedComic.series.name + "\r\n";
            }
        }

        public string Dates
        {
            get
            {
                if (SelectedComic == null) return string.Empty;
                if (SelectedComic.dates.Length < 1) return string.Empty;

                string summary = string.Empty;
                foreach (var item in SelectedComic.dates)
                {
                    summary = summary + item.date + "\r\n";
                }
                return summary.Replace("<br>", string.Empty);
            }
        }

        protected void GetImageAsync()
        {
            string imageURL = string.Empty;

            if (SelectedComic?.images.Length > 0)
            {
                imageURL = SelectedComic?.images[0].path + "." + SelectedComic?.images[0].extension;
            }
            else if (SelectedComic?.thumbnail != null)
            {
                imageURL = SelectedComic.thumbnail.path + "." + SelectedComic.thumbnail.extension;
            }

            Image = new System.Windows.Controls.Image();

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imageURL, UriKind.Absolute);
            bitmap.EndInit();

            Image.Source = bitmap;
        }

        public System.Windows.Controls.Image Image { get; set; }
    }
}
