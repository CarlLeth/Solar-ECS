using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Infrastructure
{
    public class ConversionStrippingExpressionVisitor : ExpressionVisitor
    {
        private IEnumerable<Type> ConversionsToStrip;

        public ConversionStrippingExpressionVisitor(params Type[] conversionsToStrip)
        {
            this.ConversionsToStrip = conversionsToStrip;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            var isConversion = node.NodeType == ExpressionType.Convert || node.NodeType == ExpressionType.ConvertChecked;

            if (isConversion && ConversionsToStrip.Contains(node.Type))
            {
                return node.Operand;
            }
            else
            {
                return base.VisitUnary(node);
            }
        }
    }
}
