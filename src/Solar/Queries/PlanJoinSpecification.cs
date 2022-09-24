using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Queries
{
    public class PlanJoinSpecification<TPlanKey, TPlanResult, TJoinKey>
    {
        public IQueryPlan<TPlanKey, TPlanResult> Plan { get; private set; }
        public Expression<Func<IKeyWith<TPlanKey, TPlanResult>, TJoinKey>> KeySelector { get; private set; }

        private IEnumerable<IKeyWith<TPlanKey, TPlanResult>> _cachedResults;

        /// <summary>
        /// Returns true if and only if a Predicate on TJoinKey would also apply to TPlanKey.
        /// </summary>
        public bool CanPassPredicateThrough { get; private set; }

        public PlanJoinSpecification(IQueryPlan<TPlanKey, TPlanResult> plan, Expression<Func<IKeyWith<TPlanKey, TPlanResult>, TJoinKey>> keySelector, bool canPassPredicateThrough)
        {
            this.Plan = plan;
            this.KeySelector = keySelector;
            this.CanPassPredicateThrough = canPassPredicateThrough;
        }

        public IEnumerable<IKeyWith<TPlanKey, TPlanResult>> Execute()
        {
            if (_cachedResults == null)
            {
                _cachedResults = Plan.ExecuteAll();
            }

            return _cachedResults;
        }
    }
}
