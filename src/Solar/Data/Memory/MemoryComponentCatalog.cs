using Solar.Ecs.Construction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Data.Memory
{
    public class MemoryComponentCatalog : IPurgeableComponentCatalog, IComponentRegistration
    {
        private Dictionary<Type, IStore> StoresByType { get; set; }

        public MemoryComponentCatalog(params IComponentPackage[] componentPackages)
        {
            StoresByType = new Dictionary<Type, IStore>();

            foreach (var package in componentPackages)
            {
                RegisterComponentPackage(package);
            }
        }

        public bool Contains(Type componentType)
        {
            return StoresByType.ContainsKey(componentType);
        }

        public IStore Store(Type componentType)
        {
            if (!StoresByType.ContainsKey(componentType))
            {
                throw new InvalidOperationException($"Component type '{componentType.Name}' has not been registered with this in-memory catalog. Is it missing registration in a Component Package?");
            }

            return (IStore)StoresByType[componentType];
        }

        public void RegisterComponentType<TComponent>()
        {
            if (!Contains(typeof(TComponent)))
            {
                AddStore(new MemoryStore<TComponent>());
            }
        }

        public void RegisterComponentPackage(IComponentPackage package)
        {
            package.Register(this);
        }

        public void AddStore<TComponent>(IStore<TComponent> store)
        {
            if (!Contains(typeof(TComponent)))
            {
                StoresByType[typeof(TComponent)] = store;
            }
            else
            {
                throw new InvalidOperationException("Catalog already contains a store for type " + typeof(TComponent).FullName);
            }
        }

        public void PermanentlyDestroyAllData()
        {
            foreach (var type in StoresByType.Keys.ToList())
            {
                StoresByType[type] = (IStore)Activator.CreateInstance(typeof(MemoryStore<>).MakeGenericType(type));
            }
        }

        void IComponentRegistration.Component<TComponent>()
        {
            RegisterComponentType<TComponent>();
        }


        void IComponentRegistration.Package(IComponentPackage package, params string[] childNamespace)
        {
            package.Register(this);
        }

        void IComponentRegistration.ValueType<TValueType>()
        {
            //Do nothing
        }


        public IEnumerable<Type> AvailableComponentTypes
        {
            get { return StoresByType.Keys; }
        }
    }
}
