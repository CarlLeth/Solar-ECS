using Fusic;
using SolarEcs.Construction.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Fusic.BuildStrategies
{
    public class QueryReductionStrategyBuildStrategy : IBuildStrategy
    {
        public bool CanBuild(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IReductionStrategy<>))
            {
                var queryType = type.GetGenericArguments()[0];
                return queryType.IsGenericType && (
                    queryType.GetGenericTypeDefinition() == typeof(IQueryPlan<>) ||
                    queryType.GetGenericTypeDefinition() == typeof(IQueryPlan<,>)
                );
            }
            else
            {
                return false;
            }
        }

        public BuildResult Build(Type type, IBuildSession buildSession)
        {
            var queryType = type.GetGenericArguments()[0];
            var genericArguments = queryType.GetGenericArguments();

            if (genericArguments.Length == 1)
            {
                return WrapTwoParamStrategyInOneParamStrategy(genericArguments, buildSession);
            }
            else
            {
                return CreateDefaultReducer(genericArguments);
            }
        }

        private BuildResult WrapTwoParamStrategyInOneParamStrategy(Type[] genericArguments, IBuildSession buildSession)
        {
            var keyType = typeof(Guid);
            var resultType = genericArguments[0];

            var queryPlanType = typeof(IQueryPlan<,>).MakeGenericType(keyType, resultType);
            var twoParamReductionStrategyBuildResult = buildSession.Build(typeof(IReductionStrategy<>).MakeGenericType(queryPlanType));

            if (twoParamReductionStrategyBuildResult.WasSuccessful)
            {
                return BuildResult.Success(() => {
                    var strategy = twoParamReductionStrategyBuildResult.BuiltObject;
                    var strategyType = typeof(EntityQueryReductionStrategyWrapper<>).MakeGenericType(resultType);
                    return Activator.CreateInstance(strategyType, strategy);
                });
            }
            else
            {
                return twoParamReductionStrategyBuildResult;
            }
        }

        private BuildResult CreateDefaultReducer(Type[] genericArguments)
        {
            return BuildResult.Success(() =>
            {
                var keyType = genericArguments[0];
                var resultType = genericArguments[1];

                return Activator.CreateInstance(typeof(UnionQueryReductionStrategy<,>).MakeGenericType(keyType, resultType));
            });
        }

        private class EntityQueryReductionStrategyWrapper<TResult> : IReductionStrategy<IQueryPlan<TResult>>
        {
            private IReductionStrategy<IQueryPlan<Guid, TResult>> BaseReductionStrategy;

            public EntityQueryReductionStrategyWrapper(IReductionStrategy<IQueryPlan<Guid, TResult>> baseReductionStrategy)
            {
                this.BaseReductionStrategy = baseReductionStrategy;
            }

            public IQueryPlan<TResult> Reduce(IEnumerable<SystemGeneratedResult<IQueryPlan<TResult>>> systemGeneratedResults)
            {
                var twoParamResults = systemGeneratedResults
                    .Select(o => new SystemGeneratedResult<IQueryPlan<Guid, TResult>>(o.SystemType, o.Result));

                return BaseReductionStrategy.Reduce(twoParamResults).AsEntityQuery();
            }
        }
    }
}
