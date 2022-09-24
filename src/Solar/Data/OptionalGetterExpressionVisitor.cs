using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Solar.Ecs.Infrastructure;

namespace Solar.Ecs.Data
{
    public class OptionalGetterExpressionVisitor : ExpressionVisitor
    {
        private static MethodInfo TemplateExpressionMethod = typeof(OptionalGetterExpressionVisitor).GetMethod(nameof(TemplateExpression), BindingFlags.NonPublic | BindingFlags.Static);
        private static MethodInfo TemplateExpressionWithDefaultMethod = typeof(OptionalGetterExpressionVisitor).GetMethod(nameof(TemplateExpressionWithDefault), BindingFlags.NonPublic | BindingFlags.Static);

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType.IsGenericType && node.Method.DeclaringType.GetGenericTypeDefinition() == typeof(Optional<>) && node.Method.Name == nameof(Optional<object>.Get))
            {
                var optionalType = node.Object.Type.GetGenericArguments()[0];
                var valueType = node.Method.ReturnType;

                Expression optional = node.Object;
                Expression getter = node.Arguments[0];
                LambdaExpression template;

                var visitor = new ParameterReplacingExpressionVisitor();

                if (node.Arguments.Count == 1)
                {
                    template = TemplateExpressionMethod.MakeGenericMethod(optionalType, valueType).Invoke(null, new object[] { }) as LambdaExpression;
                }
                else
                {
                    template = TemplateExpressionWithDefaultMethod.MakeGenericMethod(optionalType, valueType).Invoke(null, new object[] { }) as LambdaExpression;

                    Expression defaultIfEmpty = node.Arguments[1];
                    visitor.AddReplacementRule(template.Parameters[2], defaultIfEmpty);
                }

                visitor.AddReplacementRule(template.Parameters[0], optional);
                visitor.AddReplacementRule(template.Parameters[1], getter);

                var body = visitor.Visit(template.Body);
                return base.Visit(body);
            }
            else
            {
                return base.VisitMethodCall(node);
            }
        }

        private static Expression<Func<Optional<T>, Func<T, TValue>, TValue>> TemplateExpression<T, TValue>()
        {
            return (opt, getter) => opt.ValueOrEmpty.Select(getter).FirstOrDefault();
        }

        private static Expression<Func<Optional<T>, Func<T, TValue>, TValue, TValue>> TemplateExpressionWithDefault<T, TValue>()
        {
            return (opt, getter, defaultIfEmpty) => opt.ValueOrEmpty.Any() ? opt.ValueOrEmpty.Select(getter).FirstOrDefault() : defaultIfEmpty;
        }
    }
}
