using SolarEcs.Basics;
using SolarEcs.Queries;
using SolarEcs.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Recipes
{
    public class SingleApplicableCompositeRecipe<TModel> : IRecipe<TModel>
    {
        private IEnumerable<IRecipe<TModel>> Recipes;

        public SingleApplicableCompositeRecipe(IEnumerable<IRecipe<TModel>> recipes)
        {
            this.Recipes = recipes;
        }

        public IQueryPlan<TModel> ExistingModels => QueryPlan.Union(Recipes.Select(o => o.ExistingModels));

        public ITransaction<TModel> CreateTransaction()
        {
            var transactions = Recipes.Select(o => o.CreateTransaction()).ToList();

            //Create a reference to the returned transaction within the closure of the assignAction variable
            //to allow it to invalidate the transaction.
            ITransaction<TModel> transaction = null;
            Action<Guid, TModel> assignAction = (id, model) => Assign(id, model, transaction, transactions);

            var existingModels = new UnionQueryPlan<Guid, TModel>(transactions.Select(o => o.ExistingModels)).AsEntityQuery();

            Func<Guid, TModel, bool> canAssign = (id, model) => transactions.Count(t => t.CanAssign(id, model)) == 1;

            transaction = new EagerTransaction<TModel>(
                canAssign,
                assignAction,
                id => transactions.ForEach(o => o.Unassign(id)),
                () => transactions.SelectMany(o => o.ApplyChanges()).ToList().Distinct(),
                existingModels
            );

            return transaction;
        }

        private void Assign(Guid id, TModel model, ITransaction<TModel> selfTransaction, IEnumerable<ITransaction<TModel>> transactions)
        {
            var applicableRecipes = transactions.Where(o => o.CanAssign(id, model));

            if (applicableRecipes.Count() == 1)
            {
                var singleApplicable = applicableRecipes.First();
                transactions.Where(o => o != singleApplicable).ForEach(o => o.Unassign(id));
                singleApplicable.Assign(id, model);
            }
            else
            {
                selfTransaction.Invalidate();
            }
        }
    }
}
