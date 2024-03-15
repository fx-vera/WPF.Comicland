namespace Applivery.Desktop.Core.Interfaces
{
    public interface IPageManager
    {
        /// <summary>
        /// - For single instance windows: creates the single instance if not present, 
        /// or unminimizes and brings to front the existing one
        /// - For multiple instance windows: opens a new instance of the window if the maximum number has not been reached
        /// </summary>
        /// <param name="newPage">Page Content to insert in the new window</param>
        /// <param name="objectToLoad">If it is an editor, the parameter to pass to the "Load" function before opening the window</param>
        /// <param name="doNotCheckNumInstances">Create the window no matter if the editor is set to one instance or a maximum number</param>
        /// <param name="modalWindow">If the window should be opened as modal (no other window can be accessed until this new one is closed)</param>
        /// <returns>The window has been created and/or opened, or null if the operation could not be performed</returns>
        IWindowViewModel SetPageInMainWindow(IViewModel newPage);
    }
}
