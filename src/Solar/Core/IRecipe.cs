using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs
{
    public interface IRecipe<TModel>
    {
        ITransaction<TModel> CreateTransaction();
        IQueryPlan<TModel> ExistingModels { get; }
    }

    public static class IRecipeExtensions
    {
        public static void AssignCommit<TModel>(this IRecipe<TModel> recipe, Guid id, TModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            var transaction = recipe.CreateTransaction();
            if (transaction.CanAssign(id, model))
            {
                transaction.Assign(id, model);
                transaction.Commit();
            }
            else
            {
                throw new InvalidOperationException("Cannot assign the given component to the given entity using the given recipe.");
            }
        }

        public static Guid AddCommit<TModel>(this IRecipe<TModel> recipe, TModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            var id = Guid.NewGuid();
            recipe.AssignCommit(id, model);
            return id;
        }

        public static void UnassignCommit<TModel>(this IRecipe<TModel> recipe, Guid id)
        {
            var transaction = recipe.CreateTransaction();
            transaction.Unassign(id);
            transaction.Commit();
        }
    }
}
