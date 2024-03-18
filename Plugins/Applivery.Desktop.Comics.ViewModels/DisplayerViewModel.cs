using Applivery.Desktop.Comics.Logic;
using Applivery.Desktop.Comics.Models;
using Applivery.Desktop.Core.MVVM;
using PropertyChanged;
using System;
using System.Data;
using System.Linq;

namespace Applivery.Desktop.Comics.ViewModels
{
    /// <summary>
    /// Viewmodel to manage the main operations of the plugin.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class DisplayerViewModel : ViewModelBase
    {
        public DisplayerViewModel()
        {
            DisplayerModel = new DisplayerModel();
            ReadComicsFromAPI();
        }

        /// <summary>
        /// Contains the Comics data
        /// </summary>
        public DisplayerModel DisplayerModel { get; set; }

        public async void ReadComicsFromAPI()
        {
            try
            {
                DisplayerModel.CreatorComics.Clear();
                var rootObject = await ApiReader.ReadComicsFromAPI();

                var creators = (
                    from result in rootObject.data.results
                    from items in result.creators.items
                    group result by items.name into g
                    select new CreatorComicModel()
                    {
                        Name = g.Key,
                        Comics = g.ToList().Select(x => x).ToObservableCollection()
                    }).ToObservableCollection();

                foreach (var item in creators)
                {
                    DisplayerModel.CreatorComics.Add(item);
                }
                DisplayerModel.SelectedCreator = DisplayerModel.CreatorComics.FirstOrDefault();
            }
            catch (Exception ex)
            {
                // here log with Log4net or similar custom logger.
                var msg = ex.Message;
            }
        }
    }
}
