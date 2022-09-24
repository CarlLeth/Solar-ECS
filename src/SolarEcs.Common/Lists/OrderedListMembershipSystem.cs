using SolarEcs.Common.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Lists
{
    public class OrderedListMembershipSystem : IListSystem
    {
        public IQueryPlan<ListMembershipModel> Query { get; private set; }
        public IRecipe<ListMembershipModel> Recipe { get; private set; }

        public OrderedListMembershipSystem(IStore<OrderedListMembership> store)
        {
            this.Query = store.ToQueryPlan()
                .Select(o => new ListMembershipModel(o.Entity, o.List, o.Ordinal));

            this.Recipe = Query.StartRecipe()
                .IncludeSimple(store.ToRecipe(), o => new OrderedListMembership(o.Entity, o.List, o.Ordinal.Value))
                .Where(o => o.Model.Ordinal.HasValue);
        }
    }
}
