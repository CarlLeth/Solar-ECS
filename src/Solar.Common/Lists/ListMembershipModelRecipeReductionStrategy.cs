using SolarEcs.Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Lists
{
    public class ListMembershipModelRecipeReductionStrategy : IReductionStrategy<IRecipe<ListMembershipModel>>
    {
        public IRecipe<ListMembershipModel> Reduce(IEnumerable<SystemGeneratedResult<IRecipe<ListMembershipModel>>> systemGeneratedResults)
        {
            var recipes = systemGeneratedResults.Select(o => o.Result);

            return new SingleApplicableCompositeRecipe<ListMembershipModel>(recipes)
                .WithUniqueKey(o => o.List, o => o.Entity);
        }
    }
}
