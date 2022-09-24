using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Data.EntityFramework
{
    /// <summary>
    /// Responsible for mapping component-persistence types to a DbModel during the EntityFramework model creation phase.
    /// </summary>
    public interface IComponentTableMapper
    {
        void MapComponentToTable<TPersistedComponent>(string componentName, IEnumerable<string> componentNamespace, DbModelBuilder modelBuilder)
            where TPersistedComponent : class;
    }
}
