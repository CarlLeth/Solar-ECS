using SolarEcs.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs
{
    public static partial class QueryPlan
    {
        /// <summary>
        /// Converts a two-parameter (key-based) IQueryPlan into a one-parameter (entity-based) IQueryPlan.
        /// </summary>
        public static IQueryPlan<TResult> AsEntityQuery<TResult>(this IQueryPlan<Guid, TResult> query)
        {
            return new EntityQueryWrapper<TResult>(query);
        }
    }
}

namespace SolarEcs.Queries
{
    public class EntityQueryWrapper<TResult> : IOrderedQueryPlan<TResult>
    {
        private IQueryPlan<Guid, TResult> BaseQuery;

        public EntityQueryWrapper(IQueryPlan<Guid, TResult> baseQuery)
        {
            this.BaseQuery = baseQuery;
        }

        public QueryPlanState State
        {
            get { return BaseQuery.State; }
        }

        public IEnumerable<IKeyWith<Guid, TResult>> Execute(Expression<Func<Guid, bool>> predicate)
        {
            return BaseQuery.Execute(predicate);
        }

        public IQueryable<IKeyWith<Guid, TResult>> ImmaterialQuery
        {
            get { return BaseQuery.ImmaterialQuery; }
        }
    }
}
