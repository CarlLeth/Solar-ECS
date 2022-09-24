using Fusic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Construction.BuildStrategies
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
                var store = Catalog.Store(componentType);
                return WrapStoreInObservableProxy(store, componentType, buildSession);
            });
        }

        private object WrapStoreInObservableProxy(object store, Type componentType, IBuildSession buildSession)
        {
            var writeObserverImplementations = TypeService.GetImplementations(typeof(IComponentWriteObserver));
            var builtImplementations = writeObserverImplementations
                .Select(o => (IComponentWriteObserver)buildSession.BuildOrNull(o))
                .ToList()
                .Where(o => o != null);

            if (builtImplementations.Any())
            {
                return Activator.CreateInstance(typeof(WriteObservableProxyStore<>).MakeGenericType(componentType), new object[] { store, builtImplementations });
            }
            else
            {
                return store;
            }
        }
    }
}
