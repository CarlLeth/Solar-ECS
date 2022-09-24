using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.Queries
{
    public class EmptyJoinPlanImplicit<TRightKey, TJoinKey, TResult> : EmptyJoinPlan<Guid, TRightKey, TJoinKey, Guid, TResult>,
        ILeftJoinedQueryPlan<TJoinKey, TResult>, IJoinedQueryPlan<TRightKey, TJoinKey, TResult>
    {
        public new IQueryPlan<TResult> TakeLeftKey()
        {
            return QueryPlan.Empty<TResult>();
        }
    }

    public class EmptyJoinPlan<TLeftKey, TRightKey, TJoinKey, TSelectedKey, TResult> : EmptyQueryPlan<TSelectedKey, TResult>,
        IJoinedQueryPlan<TLeftKey, TRightKey, TJoinKey, TSelectedKey, TResult>,
        ILeftJoinedQueryPlan<TLeftKey, TJoinKey, TSelectedKey, TResult>
    {
        public IQueryPlan<TLeftKey, TResult> TakeLeftKey()
        {
            return new EmptyJoinPlan<TLeftKey, TRightKey, TJoinKey, TLeftKey, TResult>();
        }

        public IQueryPlan<TRightKey, TResult> TakeRightKey()
        {
            return new EmptyJoinPlan<TLeftKey, TRightKey, TJoinKey, TRightKey, TResult>();
        }

        public IQueryPlan<TJoinKey, TResult> TakeJoinKey()
        {
            return new EmptyJoinPlan<TLeftKey, TRightKey, TJoinKey, TJoinKey, TResult>();
        }
    }
}
