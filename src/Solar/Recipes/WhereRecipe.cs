using Solar.Ecs.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.Recipes
{
    public class WhereRecipe<TModel> : IRecipe<TModel>
    {
        private IRecipe<TModel> BaseRecipe;
        private Expression<Func<IKeyWith<Guid, TModel>, bool>> Predicate;
        private Lazy<Func<IKeyWith<Guid, TModel>, bool>> PredicateCompiled;

        public WhereRecipe(IRecipe<TModel> baseRecipe, Expression<Func<IKeyWith<Guid, TModel>, bool>> predicate)
        {
            this.BaseRecipe = baseRecipe;
            this.Predicate = predicate;
            this.PredicateCompiled = new Lazy<Func<IKeyWith<Guid, TModel>, bool>>(() => predicate.Compile());
        }

        public IQueryPlan<TModel> ExistingModels => BaseRecipe.ExistingModels.Where(Predicate);

        public ITransaction<TModel> CreateTransaction()
        {
            var baseTransaction = BaseRecipe.CreateTransaction();
            var existingModels = baseTransaction.ExistingModels.Where(Predicate);

            return new EagerTransaction<TModel>(
                (id, model) => PredicateCompiled.Value(KeyWith.Create(id, model)) && baseTransaction.CanAssign(id, model),
                (id, model) => Assign(id, model, baseTransaction),
                id => baseTransaction.Unassign(id),
                () => baseTransaction.ApplyChanges(),
                existingModels
            );
        }

        private void Assign(Guid id, TModel model, ITransaction<TModel> baseTransaction)
        {
            if (PredicateCompiled.Value(KeyWith.Create(id, model)))
            {
                baseTransaction.Assign(id, model);
            }
            else
            {
                baseTransaction.Invalidate($"Transaction was invalidated because a model was assigned which failed a Where condition: '{Predicate}'");
            }
        }
    }
}
