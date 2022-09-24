using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.Queries
{
    internal static class QueryPlanExpressionExtensions
    {
        public static Expression<Func<IKeyWith<TKey, TResult>, bool>> ForKeyWith<TKey, TResult>(this Expression<Func<TKey, bool>> predicate)
        {
            Expression<Func<IKeyWith<TKey, TResult>, TKey>> keySelector = o => o.Key;
            return keySelector.Into(predicate);
        }
    }
}
