using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Lists
{
    public interface IListSystem : ISystem
    {
        IQueryPlan<ListMembershipModel> Query { get; }
        IRecipe<ListMembershipModel> Recipe { get; }
    }
}
