using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Solar.Ecs.Data;
using Solar.Ecs.Infrastructure;

namespace Solar.Ecs.Queries
{
    public static class QueryExpressions
    {
        private static ExpressionVisitor ConstructorVisitor = new ImmutableConstructorSwappingExpressionVisitor();
        private static ExpressionVisitor OptionalGetterVisitor = new OptionalGetterExpressionVisitor();

        public static Expression<T> Clean<T>(this Expression<T> expr)
        {
            return Expression.Lambda<T>(Clean(expr.Body), expr.Parameters);
        }

        public static Expression Clean(this Expression expr)
 
        {
            return ConstructorSafe(OptionalSafe(expr));
        }

        public static Expression ConstructorSafe(Expression expr)
        {
            return ConstructorVisitor.Visit(expr);
        }

        public static Expression OptionalSafe(Expression expr)
        {
            return OptionalGetterVisitor.Visit(expr);
        }
    }
}
