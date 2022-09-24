using SolarEcs.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Recipes
{
    public class IncludeRecipe<TModel, TPart> : IRecipe<TModel>
    {
        private IRecipe<TModel> BaseRecipe;
        private IRecipe<TPart> RecipePart;
        private Action<ITransaction<TPart>, Guid, TModel> AssignPart;
        private Action<ITransaction<TPart>, Guid> UnassignPart;

        public IQueryPlan<TModel> ExistingModels => BaseRecipe.ExistingModels;

        public IncludeRecipe(IRecipe<TModel> baseRecipe, IRecipe<TPart> recipePart, Action<ITransaction<TPart>, Guid, TModel> assignPart, Action<ITransaction<TPart>, Guid> unassignPart)
        {
            this.BaseRecipe = baseRecipe;
            this.RecipePart = recipePart;
            this.AssignPart = assignPart;
            this.UnassignPart = unassignPart;
        }

        public ITransaction<TModel> CreateTransaction()
        {
            var baseTransaction = BaseRecipe.CreateTransaction();
            var partTransaction = RecipePart.CreateTransaction();

            return new EagerTransaction<TModel>(
                (id, model) => baseTransaction.CanAssign(id, model),
                (id, model) => Assign(baseTransaction, partTransaction, id, model),
                id => Unassign(baseTransaction, partTransaction, id),
                () => ApplyChanges(baseTransaction, partTransaction),
                baseTransaction.ExistingModels
            );
        }

        private void Assign(ITransaction<TModel> baseTransaction, ITransaction<TPart> partTransaction, Guid id, TModel model)
        {
            baseTransaction.Assign(id, model);
            AssignPart.Invoke(partTransaction, id, model);
        }

        private void Unassign(ITransaction<TModel> baseTransaction, ITransaction<TPart> partTransaction, Guid id)
        {
            baseTransaction.Unassign(id);
            UnassignPart.Invoke(partTransaction, id);
        }

        private IEnumerable<ICommitable> ApplyChanges(ITransaction<TModel> baseTransaction, ITransaction<TPart> partTransaction)
        {
            return baseTransaction.ApplyChanges()
                .Union(partTransaction.ApplyChanges());
        }
    }
}
