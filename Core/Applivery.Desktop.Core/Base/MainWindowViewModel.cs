using Applivery.Desktop.Core.Interfaces;
using Applivery.Desktop.Core.Managers;
using Applivery.Desktop.Core.Models;
using Applivery.Desktop.Core.MVVM;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;

namespace Applivery.Desktop.Core.Base
{
    [Export(typeof(IMainWindowViewModel))]
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public sealed class MainWindowViewModel : IMainWindowViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        /// <param name="pm">The pm.</param>
        [ImportingConstructor]
        public MainWindowViewModel()
        {
            Title = "Marvel Comics Displayer";
            Plugins = new ObservableCollection<PluginItemBase>();
            RegisterAssociatedView();

            IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();
            eventAggregator.Register<LoadPluginEventArgs>(this, SetPlugins, false, null);

            Id = "{EA29CDC3-073D-4C08-BE0A-7D1C9DB1A27C}";
        }

        public string Title { get; set; }

        public ObservableCollection<PluginItemBase> Plugins { get; set; }

        public string Id { get; set; }

        public IViewModel ViewModel { get; set; }

        /// <summary>
        /// Registers upon creation the type of the view associated to this view model.
        /// We do this instead of creating a ViewMapping to allow subclasses to change the view
        /// </summary>
        protected void RegisterAssociatedView()
        {
            IoC.Get<ViewsManager>().RegisterView(typeof(MainWindowViewModel), typeof(Applivery.Desktop.Core.Base.MainWindow));
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
                plugin = new PluginItemBase(args.Item.Id, args.Item.Name, args.Item.Command);
                Plugins.Add(plugin);
            }
        }
    }
}
