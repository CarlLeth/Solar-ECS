using Castle.DynamicProxy;
using Fusic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.Construction.Modifiers
{
    public class CompositeSystemInterceptor : IInterceptor
    {
        private Func<IEnumerable<ISystem>> SystemInitializer;
        private IBuildSession BuildSession;

        private IEnumerable<ISystem> Systems;

        public CompositeSystemInterceptor(Func<IEnumerable<ISystem>> systemInitializer, IBuildSession buildSession)
        {
            this.SystemInitializer = systemInitializer;
            this.BuildSession = buildSession;
        }

        private void Initialize()
        {
            if (Systems == null)
            {
                Systems = SystemInitializer().ToList();
                SystemInitializer = null;
            }
        }

        public void Intercept(IInvocation invocation)
        {
            Initialize();

            var returnType = invocation.Method.ReturnType;

            if (returnType == typeof(void))
            {
                PassInvocationToAllImplementations(invocation);
            }
            else
            {
                var getResultsMethod = this.GetType()
                    .GetMethod("GetReducedResultFromAllImplementations", BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(returnType);

                invocation.ReturnValue = getResultsMethod.Invoke(this, new object[] { invocation });
            }
        }

        private void PassInvocationToAllImplementations(IInvocation invocation)
        {
            Systems.ForEach(o => invocation.Method.Invoke(o, invocation.Arguments));
        }

        private TResult GetReducedResultFromAllImplementations<TResult>(IInvocation invocation)
        {
            var results = Systems.Select(system => new SystemGeneratedResult<TResult>(
                system.GetType(),
                (TResult)invocation.Method.Invoke(system, invocation.Arguments)
            )).ToList();

            var reductionStrategy = BuildSession.Build<IReductionStrategy<TResult>>();

            return reductionStrategy.Reduce(results);
            
        }
    }
}
