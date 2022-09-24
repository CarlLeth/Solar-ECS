using SolarEcs.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Data.Memory
{
    public class MemoryStore<TComponent> : IStore<TComponent>
    {
        private Dictionary<Guid, TComponent> KnownComponents { get; set; }
        private TComponent NullComponent { get; set; }

        public MemoryStore()
            : this(default(TComponent))
        {
        }

        public MemoryStore(TComponent nullComponent)
        {
            this.NullComponent = nullComponent;
            KnownComponents = new Dictionary<Guid, TComponent>();
        }

        public IQueryable<IEntityWith<TComponent>> All
        {
            get
            {
                return KnownComponents.Select(o => new EntityWith<TComponent>(o.Key, o.Value)).AsQueryable();
            }
        }

        public bool Contains(Guid id)
        {
            return KnownComponents.ContainsKey(id);
        }

        public ITransaction<TComponent> CreateTransaction()
        {
            return new ActionBufferingTransaction<TComponent>((id, component) => true, Assign, Unassign, Enumerable.Empty<ICommitable>(), this.ToQueryPlan());
        }

        private void Assign(Guid id, TComponent component)
        {
            if (id == default(Guid))
            {
                throw new ArgumentException(string.Format("Attempted to assign a component to the empty id '{0}'", default(Guid)));
            }

            KnownComponents[id] = component;
        }

        private void Unassign(Guid id)
        {
            if (id == default(Guid))
            {
                throw new ArgumentException(string.Format("Attempted to unassign a component from the empty id '{0}'", default(Guid)));
            }

            if (KnownComponents.ContainsKey(id))
            {
                KnownComponents.Remove(id);
            }
        }
    }
}
