using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.Lists
{
    public interface IListSystem : ISystem
    {
        IQueryPlan<ListMembershipModel> Query { get; }
        IRecipe<ListMembershipModel> Recipe { get; }
    }
}
