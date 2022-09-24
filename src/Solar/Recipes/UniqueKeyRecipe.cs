using SolarEcs.Infrastructure;
using SolarEcs.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Recipes
{
    public class UniqueKeyRecipe<TModel> : IRecipe<TModel>
    {
        private IRecipe<TModel> BaseRecipe;
        private IEnumerable<LazyCompiledExpression<Func<TModel, object>>> UniqueKeySelectors;

        public UniqueKeyRecipe(IRecipe<TModel> baseRecipe, params Expression<Func<TModel, object>>[] uniqueKeySelectors)
        {
            this.BaseRecipe = baseRecipe;
            this.UniqueKeySelectors = uniqueKeySelectors.Select(expr => new LazyCompiledExpression<Func<TModel, object>>(expr));
        }

        public IQueryPlan<TModel> ExistingModels => BaseRecipe.ExistingModels;

        public ITransaction<TModel> CreateTransaction()
        {
            var baseTransaction = BaseRecipe.CreateTransaction();

            return new EagerTransaction<TModel>(
                (id, model) => baseTransaction.CanAssign(id, model),
                (id, model) => Assign(baseTransaction, id, model),
                id => baseTransaction.Unassign(id),
                () => baseTransaction.ApplyChanges(),
                baseTransaction.ExistingModels
            );
        }

        private void Assign(ITransaction<TModel> baseTransaction, Guid id, TModel model)
        {
            var fullPredicate = CreatePredicate(id, model);
            var matches = baseTransaction.ExistingModels.Where(fullPredicate).ExecuteAll();

            foreach (var match in matches)
            {
                baseTransaction.Unassign(match.Key);
            }

            baseTransaction.Assign(id, model);
        }

        private Expression<Func<IKeyWith<Guid, TModel>, bool>> CreatePredicate(Guid id, TModel model)
        {
            Expression<Func<IKeyWith<Guid, TModel>, TModel>> modelSelector = o => o.Model;

            //This expression visitor turns Expressions returning objects into Expressions returning their
            //original type.  This is necessary for appropriate comparisons.
            var conversionStripper = new ConversionStrippingExpressionVisitor(typeof(object));

            Expression predicateBody = null;

            foreach (var selector in UniqueKeySelectors)
            {
                var key = selector.Compiled(model);

                var replacer = new ParameterReplacingExpressionVisitor();
                replacer.AddReplacementRule(selector.Expression.Parameters[0], modelSelector.Body);
                var selectorBody = replacer.Visit(selector.Expression.Body);

                var deconvertedSelectorBody = conversionStripper.Visit(selectorBody);

                var equalityComparator = Expression.Equal(deconvertedSelectorBody, Expression.Constant(key, key.GetType()));

                if (predicateBody == null)
                {
                    predicateBody = equalityComparator;
                }
                else
                {
                    predicateBody = Expression.AndAlso(predicateBody, equalityComparator);
                }
            }

            return Expression.Lambda<Func<IKeyWith<Guid, TModel>, bool>>(predicateBody, modelSelector.Parameters[0]);
        }
    }
}
