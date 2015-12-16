using System.Collections.Generic;

namespace d60.Cirqus.Views.ViewManagers
{
    /// <summary>
    /// Advanced API for a view manager
    /// </summary>
    public interface IViewManagerAdvanced
    {
        /// <summary>
        /// Returns ids of existing view instances.
        /// </summary>
        IEnumerable<string> GetViewIds();
    }
}