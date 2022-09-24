using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Recipes
{
    public class ComponentRecipe<TComponent> : IRecipe<TComponent>
    {
        private IStore<TComponent> Store;

        public ComponentRecipe(IStore<TComponent> store)
        {
            this.Store = store;
        }

        public IQueryPlan<TComponent> ExistingModels => Store.ToQueryPlan();

        public bool CanAssign(Guid id, TComponent model)
        {
            return true;
        }

        public ITransaction<TComponent> CreateTransaction()
        {
            return Store.CreateTransaction();
        }
    }
}
