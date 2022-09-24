using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar
{
    /// <summary>
    /// Provides data access to all component types.
    /// </summary>
    public interface IComponentCatalog
    {
        /// <summary>
        /// Whether or not the componentType exists in this Catalog.
        /// </summary>
        bool Contains(Type componentType);

        /// <summary>
        /// Retrieve an IStore for the componentType.
        /// Throws an exception if componentType does not exist in this Catalog.
        /// </summary>
        IStore Store(Type componentType);

        /// <summary>
        /// A list of all componentTypes available in this Catalog.
        /// </summary>
        IEnumerable<Type> AvailableComponentTypes { get; }
    }

    public interface IPurgeableComponentCatalog : IComponentCatalog
    {
        void PermanentlyDestroyAllData();
    }

    public static class IComponentCatalogExtensions
    {
        public static bool Contains<TComponent>(this IComponentCatalog catalog)
        {
            return catalog.Contains(typeof(TComponent));
        }

        public static IStore<TComponent> Store<TComponent>(this IComponentCatalog catalog)
        {
            return (IStore<TComponent>)catalog.Store(typeof(TComponent));
        }
    }
}
