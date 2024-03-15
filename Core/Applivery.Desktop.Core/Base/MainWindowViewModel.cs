using Applivery.Desktop.Core.Interfaces;
using Applivery.Desktop.Core.Managers;
using Applivery.Desktop.Core.Models;
using Applivery.Desktop.Core.MVVM;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Applivery.Desktop.Core.Base
{
    [Export(typeof(IMainWindowViewModel))]
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class MainWindowContentViewModel : IMainWindowViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly IPageManager pm;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowContentViewModel"/> class.
        /// </summary>
        /// <param name="pm">The pm.</param>
        [ImportingConstructor]
        public MainWindowContentViewModel(IPageManager pm)
        {
            Title = "Marvel Comics Enjoy";
            Icon = null;
            Plugins = new ObservableCollection<PluginItemBase>();
            this.pm = pm;
            RegisterAssociatedView();

            IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();
            eventAggregator.Register<LoadPluginEventArgs>(this, SetPlugins, false, null);

            Id = "{EA29CDC3-073D-4C08-BE0A-7D1C9DB1A27C}";
        }

        public string Title { get; set; }
        public ImageSource Icon { get; set; }

        public ObservableCollection<PluginItemBase> Plugins { get; set; }

        public string Id { get; set; }

        public IViewModel ViewModel { get; set; }

        protected virtual void OnCloseClicked()
        {
            if (MessageBox.Show($"Are you sure you want to close the application", "Close Application",
                  MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                IoC.Get<IEventAggregator>().Unregister(this);
            }
        }

        /// <summary>
        /// Registers upon creation the type of the view associated to this view model.
        /// We do this instead of creating a ViewMapping to allow subclasses to change the view
        /// </summary>
        protected virtual void RegisterAssociatedView()
        {
            IoC.Get<ViewsManager>().RegisterView(typeof(MainWindowContentViewModel), typeof(Applivery.Desktop.Core.Base.MainWindow));
        }

        public void SetSelectedPlugin()
        {
            Plugins.FirstOrDefault()?.Command.Execute(null);
        }

        private void SetPlugins(LoadPluginEventArgs args)
        {
            if (string.IsNullOrEmpty(args.Item.Id))
            {
                return;
            }

            var plugin = Plugins.FirstOrDefault(x => x.Id.Equals(args.Item.Id, StringComparison.InvariantCultureIgnoreCase));
            if (plugin != null && args.IsCommand)
            {
                Plugins.Remove(plugin);
            }
            else if (plugin == null)
            {
                if (Plugins.Count > 19)
                {
                    return;
                }
                plugin = new PluginItemBase(args.Item.Id, args.Item.Name, args.Item.Icon, args.Item.Command);
                Plugins.Add(plugin);
            }
        }
    }
}
