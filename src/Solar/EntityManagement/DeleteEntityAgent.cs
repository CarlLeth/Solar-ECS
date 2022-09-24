using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.EntityManagement
{
    public class DeleteEntityAgent : EntityVisitor, IDeleteEntityAgent
    {
        private Dictionary<Type, ITransaction> Transactions { get; set; }
        private IComponentCatalog Catalog { get; set; }

        public DeleteEntityAgent(IComponentCatalog catalog)
            : base(catalog, ComponentTraversalMode.TraverseRelations)
        {
            this.Catalog = catalog;
        }

        protected override void VisitComponent<TComponent>(Guid entity, TComponent component)
        {
            if (!Transactions.ContainsKey(typeof(TComponent)))
            {
                Transactions[typeof(TComponent)] = Catalog.Store<TComponent>().CreateTransaction();
            }

            var transaction = (ITransaction<TComponent>)Transactions[typeof(TComponent)];
            transaction.Unassign(entity);
        }

        protected override void VisitRelatedEntities<TComponent>(Guid entity, IEnumerable<EntityWith<TComponent>> relatedEntities, PropertyInfo relation)
        {
            base.VisitEntities(relatedEntities.Select(o => o.Id));
        }

        public void DeleteEntity(Guid entity)
        {
            this.Transactions = new Dictionary<Type, ITransaction>();
            base.VisitEntity(entity);

            Transactions.Values.ForEach(o => o.Commit());

            this.Transactions = null;
        }
    }
}
