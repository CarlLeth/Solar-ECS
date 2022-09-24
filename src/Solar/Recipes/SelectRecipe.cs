using Solar.Ecs.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.Recipes
{
    public class SelectRecipe<TStart, TFinal> : IRecipe<TFinal>
    {
        private IRecipe<TStart> BaseRecipe;
        private Expression<Func<TStart, TFinal>> StartToFinal;
        private Func<TFinal, TStart> FinalToStart;

        public SelectRecipe(IRecipe<TStart> baseRecipe, Expression<Func<TStart, TFinal>> startToFinal, Func<TFinal, TStart> finalToStart)
        {
            this.BaseRecipe = baseRecipe;
            this.StartToFinal = startToFinal;
            this.FinalToStart = finalToStart;
        }

        public IQueryPlan<TFinal> ExistingModels => BaseRecipe.ExistingModels.Select(StartToFinal);

        public ITransaction<TFinal> CreateTransaction()
        {
            var baseTransaction = BaseRecipe.CreateTransaction();

            return new EagerTransaction<TFinal>(
                (id, model) => baseTransaction.CanAssign(id, FinalToStart(model)),
                (id, model) => baseTransaction.Assign(id, FinalToStart(model)),
                id => baseTransaction.Unassign(id),
                () => baseTransaction.ApplyChanges(),
                baseTransaction.ExistingModels.Select(StartToFinal)
            );
        }
    }
}
