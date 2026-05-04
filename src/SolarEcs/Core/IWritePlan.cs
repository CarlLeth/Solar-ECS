using SolarEcs.Scripting;
using SolarEcs.WritePlans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolarEcs
{
    public interface IWritePlan<T>
    {
        IQueryPlan<T> ExistingModels { get; }
        IEnumerable<ICommitable> Apply(ChangeScript<T> script);
    }

    public static class WritePlan
    {
        public static IWritePlan<T> Empty<T>() => new EmptyWritePlan<T>();
    }

    public static class WritePlanExtensions
    {
        public static void ApplyCommit<T>(this IWritePlan<T> writePlan, ChangeScript<T> script)
        {
            writePlan.Apply(script).ForEach(com => com.Commit());
        }

        public static IWritePlan<T> ToWritePlan<T>(this IStore<T> store)
        {
            return new StoreWritePlan<T>(store);
        }

        public static IWritePlan<T> StartWritePlan<T>(this IQueryPlan<T> queryPlan)
        {
            return new QueryStarterWritePlan<T>(queryPlan);
        }

        public static IWritePlan<TModel> IncludeSimple<TModel, TPart>(this IWritePlan<TModel> basePlan, IWritePlan<TPart> includedPlan,
            Func<TModel, TPart> assignSelector)
        {
            return basePlan.Include(includedPlan, script => script.Select(assignSelector));
        }

        public static IWritePlan<TModel> IncludeSimple<TModel, TPart>(this IWritePlan<TModel> basePlan, IWritePlan<TPart> includedPlan,
            Func<TModel, TPart> assignSelector, Func<TModel, bool> when)
        {
            return basePlan.Include(includedPlan,
                script => script.Require(when).Select(assignSelector)
            );
        }

        public static IWritePlan<TModel> Include<TModel, TPart>(this IWritePlan<TModel> basePlan, IWritePlan<TPart> includedPlan,
            Action<ChangeScript<TModel>, MutableChangeScript<TPart>> apply)
        {
            return basePlan.Include(includedPlan, (script, part, _) => apply(script, part));
        }

        public static IWritePlan<TModel> Include<TModel, TPart>(this IWritePlan<TModel> basePlan, IWritePlan<TPart> includedPlan,
            Action<ChangeScript<TModel>, MutableChangeScript<TPart>, IQueryPlan<TPart>> applyWithExisting)
        {
            return new IncludeWritePlan<TModel, TPart>(basePlan, includedPlan, (script, existing) =>
            {
                var mutableScript = new MutableChangeScript<TPart>();
                applyWithExisting(script, mutableScript, existing);
                return mutableScript.ToChangeScript();
            });
        }

        public static IWritePlan<TModel> Include<TModel, TPart>(this IWritePlan<TModel> basePlan, IWritePlan<TPart> includedPlan,
            Func<ChangeScript<TModel>, ChangeScript<TPart>> getPartScript)
        {
            return basePlan.Include(includedPlan, (script, _) => getPartScript(script));
        }

        public static IWritePlan<TModel> Include<TModel, TPart>(this IWritePlan<TModel> basePlan, IWritePlan<TPart> includedPlan,
            Func<ChangeScript<TModel>, IQueryPlan<TPart>, ChangeScript<TPart>> getPartScript)
        {
            return new IncludeWritePlan<TModel, TPart>(basePlan, includedPlan, getPartScript);
        }
    }
}
