using Applivery.Desktop.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;

namespace Applivery.Desktop.Core.Managers
{
    /// <summary>
    /// Allows loading modules using MEF and assigns to itself and implements the IoC class functions
    /// </summary>
    public class MEFIoCManager
    {
        private MyCompositionContainer _container;
        private static readonly MEFIoCManager _instance = new MEFIoCManager();

        public static MEFIoCManager Instance { get { return _instance; } }

        public event EventHandler<ModuleLoadedEventArgs> ModuleLoaded;

        private MEFIoCManager()
        {
            SetupIoC();
        }

        private void SetupIoC()
        {
            IoC.GetInstance = GetInstance;
            IoC.SetInstance = SetInstance;
            IoC.GetInstanceByName = GetInstanceByName;
            IoC.GetAllInstances = GetAllInstances;
            IoC.BuildUp = BuildUp;
        }

        public object GetInstanceByName(string typeName)
        {
            var exports = _container.GetExportedValues<object>(typeName);

            return exports?.FirstOrDefault();
        }

        public bool SetInstance(Type serviceType, string contractName)
        {
            string contract = string.IsNullOrEmpty(contractName) ? AttributedModelServices.GetContractName(serviceType) : contractName;
            _container.ComposeExportedValue(contractName, serviceType);
            _container.ComposeParts();
            return true;
        }

        public void ComposeParts()
        {
            _container.ComposeParts();
        }

        public object GetInstance(Type serviceType, string contractName)
        {
            string contract = string.IsNullOrEmpty(contractName) ? AttributedModelServices.GetContractName(serviceType) : contractName;
            var exports = _container.GetExportedValues<object>(contract);
            if (exports == null || exports?.ToList().Count == 0)
            {
                return null;
            }
            var myexport = exports?.FirstOrDefault();

            if (myexport is UserControl)
            {

                var modelType = (UserControl)myexport;
                return modelType;
            }

            return exports?.FirstOrDefault();
        }

        public IEnumerable<object> GetAllInstances(Type serviceType, string contractName)
        {
            string contract = string.IsNullOrEmpty(contractName) ? AttributedModelServices.GetContractName(serviceType) : contractName;
            IEnumerable<object> res = _container.GetExportedValues<object>(contract);
            return res;
        }

        public void BuildUp(object instance)
        {
            _container.SatisfyImportsOnce(instance);
        }

        /// <summary>
        /// Tries to do the MEF composition of all assemblies and preloaded objects.
        /// Throws a ModuleLoadErrorException if the composition fails
        /// </summary>
        /// <param name="preloadedObjects"></param>
        /// <param name="assembliesToLoad"></param>
        public void LoadModules(Dictionary<Type, object> preloadedObjects, IEnumerable<Assembly> assembliesToLoad)
        {
            string errorDescription = string.Empty;
            List<Assembly> distinctAssemblies = assembliesToLoad.Distinct(new AssemblyNameComparer())?.ToList();

            _container = new MyCompositionContainer(new AggregateCatalog(
                            distinctAssemblies
                            .Select(x => new AssemblyCatalog(x))
                            ));


            //Resulta que el método que nos interesa (ComposeExportedValue) para
            //cargar los objetos ya creados es genérico, y no podemos llamarlo directamente 
            //a partir de nuestro diccionario de type->object (no podemos hacer ComposeExportedValue<Type>...)
            //así que tenemos que hacer una voltereta usando Reflection para crear la llamada a 
            //método genérico que nos interesa (MakeGenericMethod(nuestroTipo) devuelve ComposeExportedValue<NuestroTipo>)
            MethodInfo genericMethod = typeof(AttributedModelServices).GetMember("ComposeExportedValue")?.FirstOrDefault() as MethodInfo;
            foreach (var pair in preloadedObjects)
            {
                var composeExportedValueMethod = genericMethod.MakeGenericMethod(pair.Key);
                composeExportedValueMethod.Invoke(_container, new object[] { _container, pair.Value });
            }


            _container.OnPartLoaded += OnPartLoaded;

            try
            {
                _container.ComposeParts();
            }
            catch (CompositionException ce)
            {
            }
            catch (Exception ex)
            {
                errorDescription = "Error loading modules with MEF: " + ex.ToString();
            }
            string log = "Module composition finished.";
            log += "\r\nParts loaded: " + string.Join(", ", _container.ParsedParts);
            Debug.WriteLine(log);
        }

        private void OnPartLoaded(string name)
        {
            if (ModuleLoaded != null)
                ModuleLoaded(this, new ModuleLoadedEventArgs(name));
        }

        class AssemblyNameComparer : IEqualityComparer<Assembly>
        {
            public bool Equals(Assembly x, Assembly y)
            {
                return x == y || x.GetName().Name == y.GetName().Name;
            }

            public int GetHashCode(Assembly obj)
            {
                return obj.GetHashCode();
            }
        }
    }

    /// <summary>
    /// CompositionContainer que lanza un evento cada vez que 
    /// carga una "parte" MEF, para tener idea del progreso de la composición
    /// </summary>
    internal class MyCompositionContainer : CompositionContainer
    {
        public delegate void PartLoadedDlg(string name);
        public event PartLoadedDlg OnPartLoaded;

        public MyCompositionContainer(ComposablePartCatalog catalog, params ExportProvider[] providers)
            : base(catalog, providers)
        {
        }


        private List<string> _parsed = new List<string>();
        public List<string> ParsedParts { get { return _parsed; } }

        protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
        {
            if (!_parsed.Contains(definition.ContractName))
            {
                _parsed.Add(definition.ContractName);
                if (OnPartLoaded != null)
                {
                    OnPartLoaded(definition.ContractName);
                }
            }

            return base.GetExportsCore(definition, atomicComposition);
        }
    }
}
