using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.LookupLists
{
    public interface ILookupListSystem : ISystem
    {
        IQueryPlan<LookupListModel> QueryFor(Guid list);
        IRecipe<LookupListModel> RecipeFor(Guid list);
    }
}
