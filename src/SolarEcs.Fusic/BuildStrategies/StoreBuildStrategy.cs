using Fusic;
using SolarEcs.Construction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Fusic.BuildStrategies
{
    public class StoreBuildStrategy : IBuildStrategy
    {
        private IComponentCatalog Catalog { get; set; }
        private ITypeService TypeService { get; set; }

        public StoreBuildStrategy(IComponentCatalog catalog, ITypeService typeService)
        {
            this.Catalog = catalog;
            this.TypeService = typeService;
        }

        public bool CanBuild(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IStore<>);
        }

        public BuildResult Build(Type type, IBuildSession buildSession)
        {
            return BuildResult.Success(() =>
            {
                var componentType = type.GetGenericArguments()[0];
                return Catalog.Store(componentType);
            });
        }
    }
}
