using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Apworks.ObjectContainers.Unity
{
    /// <summary>
    /// Represents the holder of instance items which are managed by the <c>WcfPerRequestLifetimeManager</c>
    /// lifetime manager.
    /// </summary>
    internal class InstanceItems
    {
        #region Private Fields
        private readonly Dictionary<object, object> items = new Dictionary<object, object>();
        #endregion

        #region Private Methods
        private void CleanUp(object sender, EventArgs e)
        {
            foreach (var item in items.Select(item => item.Value))
            {
                if (item is IDisposable)
                    ((IDisposable)item).Dispose();
            }
        }
        #endregion

        #region Internal Methods
        /// <summary>
        /// Finds an item by the given key.
        /// </summary>
        /// <param name="key">The key of the item.</param>
        /// <returns>The instance of the item.</returns>
        internal object Find(object key)
        {
            if (items.ContainsKey(key))
                return items[key];
            return null;
        }
        /// <summary>
        /// Sets an item with the given key.
        /// </summary>
        /// <param name="key">The key of the item.</param>
        /// <param name="value">The instance of the item.</param>
        internal void Set(object key, object value)
        {
            items[key] = value;
        }
        /// <summary>
        /// Removes the item with the given key.
        /// </summary>
        /// <param name="key">The key of the item to be removed.</param>
        internal void Remove(object key)
        {
            items.Remove(key);
        }
        
        /// <summary>
        /// Hooks the Closed and Faulted events on the <see cref="InstanceContext"/> object.
        /// </summary>
        /// <param name="instanceContext">The <see cref="InstanceContext"/> object whose
        /// Closed and Faulted events are hooked.</param>
        internal void Hook(InstanceContext instanceContext)
        {
            instanceContext.Closed += CleanUp;
            instanceContext.Faulted += CleanUp;
        }
        #endregion
    }
}
