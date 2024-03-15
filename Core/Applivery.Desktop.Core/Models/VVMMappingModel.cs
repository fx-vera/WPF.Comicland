using System;

namespace Applivery.Desktop.Core.Models
{
    public class VVMMappingModel
    {
        public Type View { get; set; }
        public Type ViewModel { get; set; }

        public VVMMappingModel() { }

        public VVMMappingModel(Type viewModel, Type view)
        {
            View = view;
            ViewModel = viewModel;
        }
    }
}
