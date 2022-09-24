using Solar.Ecs.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.Recipes
{
    public class QueryStarterRecipe<TModel> : IRecipe<TModel>
    {
        public IQueryPlan<TModel> ExistingModels { get; }

        public QueryStarterRecipe(IQueryPlan<TModel> starterQuery)
        {
            this.ExistingModels = starterQuery;
        }

        public ITransaction<TModel> CreateTransaction()
        {
            return new EagerTransaction<TModel>(
                (id, model) => true,
                (id, model) => { },
                id => { },
                () => Enumerable.Empty<ICommitable>(),
                ExistingModels
            );
        }
    }
}
