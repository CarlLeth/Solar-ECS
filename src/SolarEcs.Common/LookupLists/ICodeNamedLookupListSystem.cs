using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.LookupLists
{
    public interface ICodeNamedLookupListSystem
    {
        IQueryPlan<CodeNamedLookupListModel> QueryFor(Guid list);
        IRecipe<CodeNamedLookupListModel> RecipeFor(Guid list);
    }
}
