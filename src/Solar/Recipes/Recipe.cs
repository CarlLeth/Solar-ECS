using SolarEcs.Recipes;
using SolarEcs.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs
{
    public static class Recipe
    {
        public static IRecipe<TFinal> Select<TStart, TFinal>(this IRecipe<TStart> recipe, Expression<Func<TStart, TFinal>> startToFinal, Func<TFinal, TStart> finalToStart)
        {
            return new SelectRecipe<TStart, TFinal>(recipe, startToFinal, finalToStart);
        }

        public static IRecipe<TModel> Where<TModel>(this IRecipe<TModel> recipe, Expression<Func<IKeyWith<Guid, TModel>, bool>> predicate)
        {
            return new WhereRecipe<TModel>(recipe, predicate);
        }

        public static IRecipe<TModel> WithUniqueKey<TModel>(this IRecipe<TModel> recipe, params Expression<Func<TModel, object>>[] uniqueKeySelectors)
        {
            return new UniqueKeyRecipe<TModel>(recipe, uniqueKeySelectors);
        }

        public static IRecipe<TModel> Empty<TModel>()
        {
            return new EmptyRecipe<TModel>();
        }

        public static IRecipe<TModel> StartRecipe<TModel>(this IQueryPlan<Guid, TModel> existingModelsQuery)
        {
            return new QueryStarterRecipe<TModel>(existingModelsQuery.AsEntityQuery());
        }

        public static IRecipe<TModel> IncludeSimple<TModel, TPart>(this IRecipe<TModel> recipe, IRecipe<TPart> recipePart, Func<TModel, TPart> assignSelector)
        {
            Func<TModel, bool> defaultWhen = o => assignSelector(o) != null;
            return recipe.IncludeSimple(recipePart, assignSelector, defaultWhen);
        }

        public static IRecipe<TModel> IncludeSimple<TModel, TPart>(this IRecipe<TModel> recipe, IRecipe<TPart> recipePart, Func<TModel, TPart> assignSelector, Func<TModel, bool> when)
        {
            return recipe.Include(recipePart, 
                assign: (trans, id, model) => {
                    if (when(model))
                    {
                        trans.Assign(id, assignSelector(model));
                    }
                    else
                    {
                        trans.Unassign(id);
                    }
                },
                unassign: (trans, id) => trans.Unassign(id));
        }

        public static IRecipe<TModel> Include<TModel, TPart>(this IRecipe<TModel> recipe, IRecipe<TPart> recipePart,
            Action<ITransaction<TPart>, Guid, TModel> assign, Action<ITransaction<TPart>, Guid> unassign)
        {
            return new IncludeRecipe<TModel, TPart>(recipe, recipePart, assign, unassign);
        }

        public static IRecipe<TEnd> CreateRecipeFrom<T1, T2, TEnd>(this IQueryPlan<TEnd> existingModelsQuery, IRecipe<T1> recipe1, IRecipe<T2> recipe2,
            Func<ITransaction<T1>, ITransaction<T2>, TransactionSpec<TEnd>> transactionActionsFactory)
        {
            return new LambdaRecipe<TEnd>(existingModelsQuery, () =>
            {
                var trans1 = recipe1.CreateTransaction();
                var trans2 = recipe2.CreateTransaction();

                var actions = transactionActionsFactory(trans1, trans2);

                return new EagerTransaction<TEnd>(
                    (id, model) => true,
                    actions.Assign,
                    actions.Unassign,
                    () =>
                    {
                        return trans1.ApplyChanges()
                            .Union(trans2.ApplyChanges());
                    },
                    existingModelsQuery);
            });
        }

        public static IRecipe<TEnd> CreateRecipeFrom<T1, T2, T3, TEnd>(this IQueryPlan<TEnd> existingModelsQuery, 
            IRecipe<T1> recipe1, IRecipe<T2> recipe2, IRecipe<T3> recipe3,
            Func<ITransaction<T1>, ITransaction<T2>, ITransaction<T3>, TransactionSpec<TEnd>> transactionActionsFactory)
        {
            return new LambdaRecipe<TEnd>(existingModelsQuery, () =>
            {
                var trans1 = recipe1.CreateTransaction();
                var trans2 = recipe2.CreateTransaction();
                var trans3 = recipe3.CreateTransaction();

                var actions = transactionActionsFactory(trans1, trans2, trans3);

                return new EagerTransaction<TEnd>(
                    (id, model) => true,
                    actions.Assign,
                    actions.Unassign,
                    () =>
                    {
                        return trans1.ApplyChanges()
                            .Union(trans2.ApplyChanges())
                            .Union(trans3.ApplyChanges());
                    },
                    existingModelsQuery
                );
            });
        }
    }
}
