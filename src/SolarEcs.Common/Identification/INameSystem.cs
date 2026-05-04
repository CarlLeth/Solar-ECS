using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Identification
{
    public interface INameSystem
    {
        IQueryPlan<NameModel> Query { get; }
        IRecipe<NameModel> Recipe { get; }
        IWritePlan<NameModel> WritePlan { get; }
    }

    public static class NameSystemExtensions
    {
        public static IQueryPlan<Named<TModel>> AttachTo<TModel>(this INameSystem nameSystem, IQueryPlan<TModel> query)
        {
            return nameSystem.Query.EntityJoin(query, (name, model) => new Named<TModel>(name.Name, model));
        }

        public static IRecipe<Named<TModel>> AttachTo<TModel>(this INameSystem nameSystem, IRecipe<TModel> recipe)
        {
            return nameSystem.AttachTo(recipe.ExistingModels).StartRecipe()
                .IncludeSimple(nameSystem.Recipe, o => new NameModel(o.Name))
                .IncludeSimple(recipe, o => o.Model);
        }

        public static IWritePlan<Named<TModel>> AttachTo<TModel>(this INameSystem nameSystem, IWritePlan<TModel> writePlan)
        {
            return nameSystem.AttachTo(writePlan.ExistingModels).StartWritePlan()
                .IncludeSimple(nameSystem.WritePlan, o => new NameModel(o.Name))
                .IncludeSimple(writePlan, o => o.Model);
        }
    }
}
