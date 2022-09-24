using Fusic;
using Fusic.Common;
using SolarEcs.Construction.BuildStrategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Construction
{
    public class SolarBuildStrategies : BuildStrategyChain, IRegisterImplementations
    {
        private ITypeService TypeService;
        private CommonBuildStrategies CommonStrategies;

        public SolarBuildStrategies(Func<IComponentCatalog> catalogFactory, ITypeService typeService)
        {
            this.TypeService = typeService;
            this.CommonStrategies = new CommonBuildStrategies();

            this.RegisterFactoryMethod<IComponentCatalog>(catalogFactory);
            this.RegisterInstance<ITypeService>(typeService);
            this.RegisterFactoryMethod<IFusicContainer>(() => this.ToContainer());

            AddDefaultBuildStrategies();
            AddDefaultNamespaceExclusions();
        }

        private void AddDefaultBuildStrategies()
        {
            AddChain(CommonStrategies);

            AddBootstrapped<StoreBuildStrategy>();
            AddBootstrapped<ComponentSetBuildStrategy>();
            AddInstance(new CompositeSystemBuildStrategy(TypeService));
            AddInstance(new DependencyFactoryBuildStrategy());
            AddBootstrapped<EntityQueryBuildStrategy>();
            AddInstance(new QueryReductionStrategyBuildStrategy());
            AddInstance(new RecipeReductionStrategyBuildStrategy());
        }

        private void AddDefaultNamespaceExclusions()
        {
            ExcludeNamespace("System.Web.Http");
            ExcludeNamespace("System.Web.Mvc");
            ExcludeNamespace("ASP");
        }

        public void RegisterFactoryMethod(Type requestedType, Func<IBuildSession, object> factoryMethod)
        {
            CommonStrategies.RegisterFactoryMethod(requestedType, factoryMethod);
        }

        public void ExcludeNamespace(string namespaceToExclude)
        {
            CommonStrategies.ExcludeNamespace(namespaceToExclude);
        }
    }
}
