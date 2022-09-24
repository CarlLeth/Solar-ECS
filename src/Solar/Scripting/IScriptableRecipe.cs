using Solar.Ecs.Scripting;
using Solar.Ecs.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.Scripting
{
    public interface IScriptableRecipe<TModel>
    {
        // Applies the given script to the recipe.
        ITransaction Apply(ChangeScript<TModel> script);
    }
}

namespace Solar
{
    public static class ScriptableRecipeExtensions
    {
        public static IScriptableRecipe<TModel> AsScriptable<TModel>(this IRecipe<TModel> recipe)
        {
            return new BasicScriptableRecipe<TModel>(recipe);
        }

        public static IScriptableRecipe<TModel> Include<TModel, TInclude>(this IScriptableRecipe<TModel> baseRecipe, IRecipe<TInclude> includedRecipe, Action<ITransaction<TInclude>, IEnumerable<IKeyWith<Guid, TModel>>, IEnumerable<Guid>> action)
        {
            return new IncludeScriptableRecipe<TModel, TInclude>(baseRecipe, includedRecipe, action);
        }

        public static void ApplyCommit<TModel>(this IScriptableRecipe<TModel> recipe, ChangeScript<TModel> script)
        {
            recipe.Apply(script).Commit();
        }

        public static void ApplyCommit<TModel>(this IScriptableRecipe<TModel> recipe, IReadOnlyDictionary<Guid, TModel> assignments)
        {
            if (assignments.Count == 0)
            {
                return;
            }

            var script = new ChangeScript<TModel>(assignments, Enumerable.Empty<Guid>());
            recipe.ApplyCommit(script);
        }

        public static void ApplyCommit<T>(this IRecipe<T> recipe, ChangeScript<T> script)
        {
            recipe.AsScriptable().ApplyCommit(script);
        }


        public static ITransaction Apply<TModel>(this IScriptableRecipe<TModel> recipe, IReadOnlyDictionary<Guid, TModel> assignments)
        {
            if (assignments.Count == 0)
            {
                return Transaction.Empty();
            }

            var script = new ChangeScript<TModel>(assignments, Enumerable.Empty<Guid>());
            return recipe.Apply(script);
        }
    }
}
