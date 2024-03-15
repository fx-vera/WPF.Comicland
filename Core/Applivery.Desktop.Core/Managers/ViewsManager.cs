using Applivery.Desktop.Core.Interfaces;
using Applivery.Desktop.Core.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

namespace Applivery.Desktop.Core.Managers
{
    /// <summary>
    /// ViewsManager
    /// </summary>
    [Export(typeof(ViewsManager))]
    public class ViewsManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewsManager"/> class.
        /// </summary>
        /// <param name="views">The views.</param>
        [ImportingConstructor]
        public ViewsManager([ImportMany] IEnumerable<IVVMMappingBase> views, [Import] IPageManager pm)
        {
            _availableViews.AddRange(views);
        }

        /// <summary>
        /// The available views
        /// </summary>
        private List<IVVMMappingBase> _availableViews = new List<IVVMMappingBase>();

        /// <summary>
        /// Gets the available views.
        /// </summary>
        /// <value>
        /// The available views.
        /// </value>
        public List<IVVMMappingBase> AvailableViews { get { return _availableViews; } }

        /// <summary>
        /// Loads the availiable views.
        /// </summary>
        public void LoadAvailableViews()
        {
            AvailableViews.ForEach(v => RegisterView(v, false));

            // ahora me ahorro el [Export] de Pages
            foreach (var view in AvailableViews)
            {
                foreach (var mapping in view.Mappings)
                {
                    IoC.SetInstance(typeof(UserControl), mapping.View.Name);
                }
            }

            MEFIoCManager.Instance.ComposeParts();
        }

        /// <summary>
        /// Registers the view.
        /// </summary>
        /// <param name="view">The view.</param>
        public void RegisterView(IVVMMappingBase view)
        {
            RegisterView(view, true);
        }

        /// <summary>
        /// Registers the view.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="view">The view.</param>
        public void RegisterView(Type viewModel, Type view)
        {
            ViewViewModelMappingBase bv = new ViewViewModelMappingBase(viewModel, view);
            RegisterView(bv);
        }

        private void RegisterView(IVVMMappingBase view, bool addToAvailable)
        {
            if (view == null)
            {
                return;
            }

            // this template allow to display the user controls in the main window

            foreach (var mapping in view.Mappings)
            {
                DataTemplate dt = DataTemplateCreator.CreateTemplateForType(mapping.ViewModel, mapping.View);
                ResourceDictionary currentResources = Application.Current.Resources;
                object key = dt.DataTemplateKey;
                if (!currentResources.Contains(key))
                    currentResources.Add(key, dt);
                else
                    currentResources[key] = dt;
            }

            if (addToAvailable)
                AvailableViews.Add(view);
        }
    }

    public static class DataTemplateCreator
    {
        private const string dataTemplateString = @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" 
            xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
            xmlns:viewModels=""clr-namespace:{0};assembly={1}"" 
            xmlns:views=""clr-namespace:{2};assembly={3}"" 
            DataType=""{{x:Type viewModels:{4}}}"" >
                <views:{5} />
            </DataTemplate>";


        /// <summary>
        /// Creates a data template for templateKey types assigning them the control 
        /// in templateValue
        /// </summary>
        /// <param name="viewModelType">Type that is used as key to retrieve the template content</param>
        /// <param name="viewType">Type of the template content, that must be a control (FrameworkElement at least)</param>
        /// <returns></returns>
        public static DataTemplate CreateTemplateForType(Type viewModelType, Type viewType)
        {
            //Versión creada por nosotros a mano que recurre a menos clases de contexto del parser xaml
            StringBuilder dataTemplateXaml = new StringBuilder();
            dataTemplateXaml.AppendFormat(dataTemplateString, viewModelType.Namespace, viewModelType.Assembly.GetName().Name,
                                                  viewType.Namespace, viewType.Assembly.GetName().Name,
                                                  viewModelType.Name,
                                                  viewType.Name);


            DataTemplate dt = null;
            try
            {
                string xaml = dataTemplateXaml.ToString();//keep this in a separate line for debugging purposes (to see the generated xaml)
                XmlReader xmlReader = XmlReader.Create(new StringReader(xaml));
                dt = XamlReader.Load(xmlReader) as DataTemplate;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error creating data template for " + viewModelType.Name + " => " + viewType.Name + ": " + ex.Message);
            }

            return dt;

            //Esta versión sacada de https://www.ikriv.com/dev/wpf/DataTemplateCreation/ sí funciona
            //const string xamlTemplate = "<DataTemplate DataType=\"{{x:Type vm:{0}}}\"><v:{1} /></DataTemplate>";
            //var xaml = String.Format(xamlTemplate, viewModelType.Name, viewType.Name, viewModelType.Namespace, viewType.Namespace);

            //var context = new ParserContext();

            //context.XamlTypeMapper = new XamlTypeMapper(new string[0]);
            //context.XamlTypeMapper.AddMappingProcessingInstruction("vm", viewModelType.Namespace, viewModelType.Assembly.FullName);
            //context.XamlTypeMapper.AddMappingProcessingInstruction("v", viewType.Namespace, viewType.Assembly.FullName);

            //context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            //context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
            //context.XmlnsDictionary.Add("vm", "vm");
            //context.XmlnsDictionary.Add("v", "v");

            //var template = (DataTemplate)XamlReader.Parse(xaml, context);
            //return template;
        }
    }
}
