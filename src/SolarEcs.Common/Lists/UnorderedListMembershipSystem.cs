using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Lists
{
    public class UnorderedListMembershipSystem : IListSystem
    {
        public IQueryPlan<ListMembershipModel> Query { get; }
        public IRecipe<ListMembershipModel> Recipe { get; }

        public UnorderedListMembershipSystem(IStore<UnorderedListMembership> store)
        {
            this.Query = store.ToQueryPlan()
                .Select(o => new ListMembershipModel(o.Entity, o.List, null));

            this.Recipe = Query.StartRecipe()
                .IncludeSimple(store.ToRecipe(), o => new UnorderedListMembership(o.Entity, o.List))
                .Where(o => !o.Model.Ordinal.HasValue);
        }
    }
}
