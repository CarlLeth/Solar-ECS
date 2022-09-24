using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.Infrastructure
{
    public class ParameterReplacingExpressionVisitor : ExpressionVisitor
    {
        private List<ParameterReplacementRule> ReplacementRules { get; set; }

        public ParameterReplacingExpressionVisitor()
        {
            ReplacementRules = new List<ParameterReplacementRule>();
        }

        public void AddReplacementRule(ParameterExpression find, Expression replacement)
        {
            ReplacementRules.Add(new ParameterReplacementRule(expr => expr == find, replacement));
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            foreach (var rule in ReplacementRules)
            {
                if (rule.ShouldReplace(node))
                {
                    return rule.Replacement;
                }
            }

            return node;
        }

        private class ParameterReplacementRule
        {
            public Func<ParameterExpression, bool> ShouldReplace { get; private set; }
            public Expression Replacement { get; private set; }

            public ParameterReplacementRule(Func<ParameterExpression, bool> shouldReplace, Expression replacement)
            {
                this.ShouldReplace = shouldReplace;
                this.Replacement = replacement;
            }
        }
    }
}
