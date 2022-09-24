using Fusic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Construction.BuildStrategies
{
    public class DependencyFactoryBuildStrategy : IBuildStrategy
    {
        public bool CanBuild(Type type)
        {
            return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IDependencyFactory<>));
        }

        public BuildResult Build(Type type, IBuildSession buildSession)
        {
            var dependencyType = type.GetGenericArguments()[0];
            return buildSession.Build(typeof(DependencyFactory<>).MakeGenericType(dependencyType));
        }
    }
}
