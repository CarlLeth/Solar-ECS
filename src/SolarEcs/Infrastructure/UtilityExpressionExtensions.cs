using SolarEcs.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs
{
    public static class UtilityExpressionExtensions
    {
        /// <summary>
        /// Substitutes this expression into the parameter of outerExpression.
        /// </summary>
        public static Expression<Func<TStart, TResult>> Into<TStart, TIntermediate, TResult>(this Expression<Func<TStart, TIntermediate>> innerExpression, Expression<Func<TIntermediate, TResult>> outerExpression)
        {
            var param = innerExpression.Parameters[0];

            var parameterReplacer = new ParameterReplacingExpressionVisitor();
            parameterReplacer.AddReplacementRule(outerExpression.Parameters[0], innerExpression.Body);
            
            var body = parameterReplacer.Visit(outerExpression.Body);

            return Expression.Lambda<Func<TStart, TResult>>(body, param);
        }
    }
}
