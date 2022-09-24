using SolarEcs.Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Construction.Modifiers
{
    public class SingleApplicableRecipeReductionStrategy<TModel> : IReductionStrategy<IRecipe<TModel>>
    {
        public IRecipe<TModel> Reduce(IEnumerable<SystemGeneratedResult<IRecipe<TModel>>> systemGeneratedResults)
        {
            return new SingleApplicableCompositeRecipe<TModel>(systemGeneratedResults.Select(o => o.Result).ToList());
        }
    }
}
