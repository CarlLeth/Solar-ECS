using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.Infrastructure
{
    public class LazyCompiledExpression<TDelegate>
    {
        public Expression<TDelegate> Expression { get; private set; }

        private Lazy<TDelegate> _compiled;
        public TDelegate Compiled
        {
            get
            {
                return _compiled.Value;
            }
        }


        public LazyCompiledExpression(Expression<TDelegate> expression)
        {
            this.Expression = expression;
            this._compiled = new Lazy<TDelegate>(() => expression.Compile());
        }
    }
}
