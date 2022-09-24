using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Infrastructure
{
    public class ImmutableConstructorSwappingExpressionVisitor : ExpressionVisitor
    {
        private static readonly HashSet<Type> ExemptTypes = new HashSet<Type>()
        {
            typeof(DateTime),
            typeof(DateTime?)
        };

        protected override Expression VisitNew(NewExpression node)
        {
            if (ExemptTypes.Contains(node.Type))
            {
                return node;
            }
            else if (node.Constructor.GetParameters().Length == 0)
            {
                return node;
            }
            else if (IsAnonymousType(node.Type))
            {
                var visitedArguments = node.Arguments.Select(o => base.Visit(o));
                return Expression.New(node.Constructor, visitedArguments, node.Members);
            }

            var defaultConstructor = node.Type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null);

            if (defaultConstructor == null)
            {
                throw new InvalidOperationException(string.Format("Type '{0}' does not have a default constructor.", node.Type));
            }

            return Expression.MemberInit(Expression.New(defaultConstructor), CreateMemberBindings(node));
        }

        private bool IsAnonymousType(Type type)
        {
            return type.Namespace == null;
        }

        private IEnumerable<MemberBinding> CreateMemberBindings(NewExpression newExpression)
        {
            var constructorParams = newExpression.Constructor.GetParameters();

            for (int i = 0; i < constructorParams.Length; i++)
            {
                var param = constructorParams[i];
                var argument = base.Visit(newExpression.Arguments[i]);

                yield return CreateMemberBinding(newExpression.Type, param, argument);
            }
        }

        private MemberBinding CreateMemberBinding(Type type, ParameterInfo originalParameter, Expression argument)
        {
            var property = type.GetProperty(ToPascalCase(originalParameter.Name));
            if (property == null)
            {
                throw new InvalidOperationException(string.Format("Could not find expected property named '{0}' based on constructor parameter named '{1}'.",
                    ToPascalCase(originalParameter.Name), originalParameter.Name));
            }

            return Expression.Bind(property, argument);
        }

        private string ToPascalCase(string camelCase)
        {
            return string.Format("{0}{1}", camelCase.Substring(0, 1).ToUpper(), camelCase.Substring(1));
        }
    }
}
