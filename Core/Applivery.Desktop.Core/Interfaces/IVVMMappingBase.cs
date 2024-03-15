using Applivery.Desktop.Core.Models;
using System.Collections.Generic;

namespace Applivery.Desktop.Core.Interfaces
{
    /// <summary>
    /// Interface to be exported through MEF to register 
    /// the Views to be assigned to ViewModels
    /// </summary>
    public interface IVVMMappingBase
    {
        List<VVMMappingModel> Mappings { get; }
    }
}
