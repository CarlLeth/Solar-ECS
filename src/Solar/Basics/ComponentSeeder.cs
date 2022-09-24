using SolarEcs.Basics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Basics
{
    public class ComponentSeeder
    {
        public IComponentCatalog Catalog { get; private set; }

        public ComponentSeeder(IComponentCatalog catalog)
        {
            this.Catalog = catalog;
        }

        public IStore<TComponent> Store<TComponent>()
        {
            return Catalog.Store<TComponent>();
        }

        public Guid Add<TComponent>(TComponent component)
        {
            return Store<TComponent>().AddCommit(component);
        }

        public void Assign<TComponent>(Guid entity, TComponent component)
        {
            Store<TComponent>().AssignCommit(entity, component);
        }

        public void Assign<TComponent>(IEntity entity, TComponent component)
        {
            Store<TComponent>().AssignCommit(entity, component);
        }
    }
}