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
            return new EmptyTransaction();
        }

        private class EmptyTransaction : ITransaction<TModel>
        {
            public EmptyTransaction()
            {
                IsValid = true;
            }

            public bool CanAssign(Guid id, TModel model)
            {
                return true;
            }

            public void Assign(Guid id, TModel component)
            {
                //Do nothing
            }

            public void Unassign(Guid id)
            {
                //Do nothing
            }

            public IEnumerable<ICommitable> ApplyChanges()
            {
                return Enumerable.Empty<ICommitable>();
            }

            public bool IsValid { get; private set; }

            public void Invalidate()
            {
                IsValid = false;
            }

            public IQueryPlan<TModel> ExistingModels
            {
                get { return QueryPlan.Empty<TModel>(); }
            }
        }
    }
}
