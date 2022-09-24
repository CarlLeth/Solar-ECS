using Solar.Ecs.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.Recipes
{
    public class EmptyRecipe<TModel> : IRecipe<TModel>
    {
        public IQueryPlan<TModel> ExistingModels => QueryPlan.Empty<TModel>();

        public ITransaction<TModel> CreateTransaction()
        {
            return Transaction.Empty<TModel>();
        }
    }
}
