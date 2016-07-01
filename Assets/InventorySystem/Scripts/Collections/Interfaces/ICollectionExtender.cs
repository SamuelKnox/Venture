using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devdog.InventorySystem
{
    public interface ICollectionExtender
    {
        /// <summary>
        /// The collection being extended.
        /// </summary>
        ItemCollectionBase extendingCollection { get; }

        /// <summary>
        /// The collection that is extending the extendingCollection
        /// </summary>
        ItemCollectionBase extenderCollection { get; }
    }
}
