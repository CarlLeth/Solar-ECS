using Fusic;
using SolarEcs;
using SolarEcs.Construction;
using SolarEcs.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Fusic.BuildStrategies
{
    public class EntityQueryBuildStrategy : IBuildStrategy
    {
        private IComponentCatalog Catalog { get; set; }

        public EntityQueryBuildStrategy(IComponentCatalog catalog)
        {
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
