using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Data.EntityFramework
{
    /// <summary>
    /// Maps a component type to a corresponding persistence type for use in EntityFramework models.
    /// </summary>
    public interface IPersistenceTypeLibrary
    {
        Type GetPersistedType(Type componentType);
        void CreatePersistenceType(Type componentType);
    }

    public static class IPersistenceTypeLibraryExtensions
    {
        public static Type GetPersistedType<TComponent>(this IPersistenceTypeLibrary library)
        {
            return library.GetPersistedType(typeof(TComponent));
        }
    }
}