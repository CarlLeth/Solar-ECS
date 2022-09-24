using Fusic;
using Solar.Ecs.Construction.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.Construction.BuildStrategies
{
    public class RecipeReductionStrategyBuildStrategy : IBuildStrategy
    {
        public bool CanBuild(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IReductionStrategy<>))
            {
                var recipeType = type.GetGenericArguments()[0];
                return recipeType.IsGenericType && recipeType.GetGenericTypeDefinition() == typeof(IRecipe<>);
            }
            else
            {
                return false;
            }
        }

        public BuildResult Build(Type type, IBuildSession buildSession)
        {
            return BuildResult.Success(() =>
            {
                var recipeType = type.GetGenericArguments()[0];
                var modelType = recipeType.GetGenericArguments()[0];

                return Activator.CreateInstance(typeof(SingleApplicableRecipeReductionStrategy<>).MakeGenericType(modelType));
            });
        }
    }
}
