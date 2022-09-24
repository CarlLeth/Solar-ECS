using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Data.EntityFramework
{
    /// <summary>
    /// A ComponentDbContext with the additional ability to destroy all data in the database, for testing, development, and sandbox purposes.
    /// </summary>
    public class SandboxComponentDbContext : ComponentDbContext, IPurgeableComponentCatalog
    {
        public SandboxComponentDbContext(string nameOrConnectionString, IPersistenceTypeLibrary persistenceTypeLibrary, IComponentTableMapper componentTableMapper)
            : base(nameOrConnectionString, persistenceTypeLibrary, componentTableMapper)
        { }

        public SandboxComponentDbContext(string nameOrConnectionString, IPersistenceTypeLibrary persistenceTypeLibrary)
            : base(nameOrConnectionString, persistenceTypeLibrary)
        { }

        public SandboxComponentDbContext(IPersistenceTypeLibrary persistenceTypeLibrary, IComponentTableMapper componentTableMapper)
            : base(persistenceTypeLibrary, componentTableMapper)
        { }

        public SandboxComponentDbContext(IPersistenceTypeLibrary persistenceTypeLibrary)
            : base(persistenceTypeLibrary)
        { }

        public void PermanentlyDestroyAllData()
        {
            foreach (var componentType in RegisteredComponentTypes)
            {
                var persistedType = PersistenceTypeLibrary.GetPersistedType(componentType);
                var components = this.Set(persistedType);
                this.Set(persistedType).RemoveRange(components);
            }

            this.SaveChanges();
        }
    }
}
