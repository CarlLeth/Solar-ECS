using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Queries
{
    public class EntityComponentQueryPlan<TComponent> : IQueryPlan<TComponent>
    {
        private IStore<TComponent> Store { get; set; }

        public EntityComponentQueryPlan(IStore<TComponent> store)
        {
            this.Store = store;
        }

        public IEnumerable<IKeyWith<Guid, TComponent>> Execute(Expression<Func<Guid, bool>> predicate)
        {
            return ImmaterialQuery.Where(predicate.ForKeyWith<Guid, TComponent>()).ToList();
        }

        public IQueryable<IKeyWith<Guid, TComponent>> ImmaterialQuery
        {
            get
            {
                return Store.All.Select(o => new KeyWith<Guid, TComponent>() { Key = o.Id, Model = o.Component });
            }
        }

        public QueryPlanState State
        {
            get { return QueryPlanState.Immaterial; }
        }
    }
}
