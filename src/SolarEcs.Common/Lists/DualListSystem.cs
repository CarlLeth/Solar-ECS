using System;
using System.Collections.Generic;
using System.Text;

namespace SolarEcs.Common.Lists
{
    /// <summary>
    /// Implementation of IListSystem that splits membership models into different components based on whether they're ordered or not.
    /// </summary>
    public class DualListSystem : IListSystem
    {
        readonly IStore<OrderedListMembership> OrderedMembers;
        readonly IStore<UnorderedListMembership> UnorderedMembers;

        public DualListSystem(IStore<OrderedListMembership> orderedMembers, IStore<UnorderedListMembership> unorderedMembers)
        {
            OrderedMembers = orderedMembers;
            UnorderedMembers = unorderedMembers;
        }

        public IQueryPlan<ListMembershipModel> Query
        {
            get
            {
                var unorderedQuery = UnorderedMembers.ToQueryPlan()
                    .Select(o => new ListMembershipModel(o.Entity, o.List, null));

                var orderedQuery = OrderedMembers.ToQueryPlan()
                    .Select(o => new ListMembershipModel(o.Entity, o.List, o.Ordinal));

                return unorderedQuery.Concat(orderedQuery);
            }
        }

        public IRecipe<ListMembershipModel> Recipe
        {
            get
            {
                return Query.StartRecipe()
                    .IncludeSimple(UnorderedMembers.ToRecipe(), model => new UnorderedListMembership(model.Entity, model.List), model => model.Ordinal == null)
                    .IncludeSimple(OrderedMembers.ToRecipe(), model => new OrderedListMembership(model.Entity, model.List, model.Ordinal.Value), model => model.Ordinal.HasValue);
            }
        }
    }
}
