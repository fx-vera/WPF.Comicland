using Applivery.Desktop.Core.Interfaces;
using System;

namespace Applivery.Desktop.Core.Models
{
    public class LoadPluginEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is favourite button; otherwise, <c>false</c>.
        /// </value>
        public bool IsCommand { get; set; }

        public IPluginItem Item { get; set; }
    }
}
