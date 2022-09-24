using Fusic;
using SolarEcs;
using SolarEcs.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Construction.BuildStrategies
{
    public class EntityQueryBuildStrategy : IBuildStrategy
    {
        private ITypeService TypeService { get; set; }
        private IComponentCatalog Catalog { get; set; }

        public EntityQueryBuildStrategy(ITypeService typeService, IComponentCatalog catalog)
        {
            this.TypeService = typeService;
            this.Catalog = catalog;
        }

        public bool CanBuild(Type type)
        {
            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(IQueryPlan<>))
            {
                return false;
            }

            var componentType = type.GetGenericArguments()[0];

            return Catalog.Contains(componentType);
        }

        public BuildResult Build(Type type, IBuildSession buildSession)
        {
            var componentType = type.GetGenericArguments()[0];
            return buildSession.Build(typeof(EntityComponentQueryPlan<>).MakeGenericType(componentType));
        }
    }
}
