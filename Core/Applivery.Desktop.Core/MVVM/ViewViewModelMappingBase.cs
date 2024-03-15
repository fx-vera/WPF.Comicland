using Applivery.Desktop.Core.Interfaces;
using Applivery.Desktop.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Applivery.Desktop.Core.MVVM
{
    /// <summary>
    /// Base class used to register in the global application dictionary a mapping between 
    /// a specific viewmodel (not the interface it implements) and a view. 
    /// </summary>
    [Export(typeof(IVVMMappingBase))]
    public class ViewViewModelMappingBase : IVVMMappingBase
    {
        private List<VVMMappingModel> _mappings = new List<VVMMappingModel>();
        public List<VVMMappingModel> Mappings { get { return _mappings; } }

        public ViewViewModelMappingBase() { }

        public ViewViewModelMappingBase(VVMMappingModel vm)
        {
            _mappings.Add(vm);
        }

        public ViewViewModelMappingBase(Type viewModel, Type view)
        {
            AddMapping(viewModel, view);
        }

        public void AddMapping(Type viewModel, Type view)
        {
            _mappings.Add(new VVMMappingModel(viewModel, view));
        }
    }

    public class ViewViewModelMappingBase<ViewModel, View> : ViewViewModelMappingBase
    {
        public ViewViewModelMappingBase() : base(typeof(ViewModel), typeof(View)) { }
    }
}
