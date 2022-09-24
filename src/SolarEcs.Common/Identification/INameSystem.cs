using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Identification
{
    public interface INameSystem : ISystem
    {
        IQueryPlan<NameModel> Query { get; }
        IRecipe<NameModel> Recipe { get; }
    }

    public static class NameSystemExtensions
    {
        public static IQueryPlan<Named<TModel>> AttachTo<TModel>(this INameSystem nameSystem, IQueryPlan<TModel> query)
        {
            return nameSystem.Query.EntityJoin(query, (name, model) => new Named<TModel>(name.Name, model));
        }

        public static IRecipe<Named<TModel>> AttachTo<TModel>(this INameSystem nameSystem, IRecipe<TModel> recipe)
        {
            return QueryPlan.Empty<Named<TModel>>().StartRecipe()
                .IncludeSimple(nameSystem.Recipe, o => new NameModel(o.Name))
                .IncludeSimple(recipe, o => o.Model);
        }
    }
}
