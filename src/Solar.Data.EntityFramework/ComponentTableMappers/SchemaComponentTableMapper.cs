using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Data.EntityFramework.ComponentTableMappers
{
    /// <summary>
    /// Maps a component to a table in a schema based on the top-level namespace.
    /// Additional child namespaces are added as a prefix joined together with a separator.
    /// </summary>
    public class SchemaComponentTableMapper : IComponentTableMapper
    {
        public string NamespaceSeparator { get; private set; }

        public SchemaComponentTableMapper()
        {
            this.NamespaceSeparator = "_";
        }

        public SchemaComponentTableMapper(string namespaceSeparator)
        {
            this.NamespaceSeparator = namespaceSeparator;
        }

        public void MapComponentToTable<TPersistedComponent>(string componentName, IEnumerable<string> componentNamespace, DbModelBuilder modelBuilder)
            where TPersistedComponent : class
        {
            string schema = PascalToCamelCase(componentNamespace.First());
            string fullName = String.Join(NamespaceSeparator, componentNamespace.Skip(1).Concat(Enumerable.Repeat(componentName, 1)));
            modelBuilder.Entity<TPersistedComponent>().ToTable(fullName, schema);
        }

        private string PascalToCamelCase(string pascalCased)
        {
            if (string.IsNullOrEmpty(pascalCased))
            {
                return pascalCased;
            }
            else
            {
                return string.Format("{0}{1}", pascalCased.ToLower()[0], pascalCased.Substring(1));
            }
        }
    }
}
