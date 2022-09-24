using Castle.DynamicProxy;
using Fusic;
using SolarEcs.Construction.Modifiers;
using SolarEcs.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Construction.BuildStrategies
{
    public class CompositeSystemBuildStrategy : IBuildStrategy
    {
        private static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();

        private ITypeService TypeService { get; set; }

        public CompositeSystemBuildStrategy(ITypeService typeService)
        {
            this.TypeService = typeService;
        }

        public bool CanBuild(Type type)
        {
            return type.IsAbstract && typeof(ISystem).IsAssignableFrom(type) && typeof(ISystem) != type && TypeService.GetImplementations(type).Count() > 1;
        }

        public BuildResult Build(Type type, IBuildSession buildSession)
        {
            return BuildResult.Success(() =>
            {
                var implementations = TypeService.GetImplementations(type);

                //Building the concrete types is deferred to allow recursive systems
                var subsystems = implementations.Select(o => buildSession.BuildOrNull(o)).Where(o => o != null).Cast<ISystem>();

                var interceptor = new CompositeSystemInterceptor(() => subsystems.ToList(), buildSession);
                return ProxyGenerator.CreateInterfaceProxyWithoutTarget(type, interceptor);
            });
        }
    }
}
