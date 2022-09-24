using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Recipes
{
    public class LambdaRecipe<TModel> : IRecipe<TModel>
    {
        public IQueryPlan<TModel> ExistingModels { get; }

        private Func<ITransaction<TModel>> TransactionFactory;

        public LambdaRecipe(IQueryPlan<TModel> existingModels, Func<ITransaction<TModel>> transactionFactory)
        {
            this.ExistingModels = existingModels;
            this.TransactionFactory = transactionFactory;
        }

        public ITransaction<TModel> CreateTransaction()
        {
            return TransactionFactory();
        }
    }
}
