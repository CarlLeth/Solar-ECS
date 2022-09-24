using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Data.EntityFramework.ComponentTableMappers
{
    /// <summary>
    /// Maps a component to a table prefixed with all its namespaces joined together with a separator.
    /// </summary>
    public class PrefixComponentTableMapper : IComponentTableMapper
    {
        public string NamespaceSeparator { get; private set; }

        public PrefixComponentTableMapper()
        {
            this.NamespaceSeparator = "_";
        }

        public PrefixComponentTableMapper(string namespaceSeparator)
        {
            this.NamespaceSeparator = namespaceSeparator;
        }

        public void MapComponentToTable<TPersistedComponent>(string componentName, IEnumerable<string> componentNamespace, DbModelBuilder modelBuilder)
             where TPersistedComponent : class
        {
            string fullName = String.Join(NamespaceSeparator, componentNamespace.Concat(Enumerable.Repeat(componentName, 1)));
            modelBuilder.Entity<TPersistedComponent>().ToTable(fullName);
        }      
    }
}
