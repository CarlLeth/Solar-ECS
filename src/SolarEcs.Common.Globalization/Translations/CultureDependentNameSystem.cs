using SolarEcs.Common.Identification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Globalization.Translations
{
    public class CultureDependentNameSystem : INameSystem
    {
        private IQueryPlan<CultureDependentName> Names;
        private StaticNameSystem Fallback;
        private ITextCulture CurrentCulture;

        public CultureDependentNameSystem(IQueryPlan<CultureDependentName> names, StaticNameSystem fallback, ITextCulture currentCulture)
        {
            this.Names = names;
            this.Fallback = fallback;
            this.CurrentCulture = currentCulture;
        }

        public IQueryPlan<NameModel> Query
        {
            get
            {
                if (!CurrentCulture.IsAvailable)
                {
                    return Fallback.Query;
                }

                var cultureSpecificNames = Names
                    .Where(o => o.Model.Culture == CurrentCulture.Id)
                    .ShiftKey(o => o.Model.Entity)
                    .Select(o => new NameModel(o.Name));

                return QueryPlan.UnionInPriorityOrder(
                    cultureSpecificNames,
                    Fallback.Query
                );
            }
        }


        public IRecipe<NameModel> Recipe
        {
            get
            {
                if (!CurrentCulture.IsAvailable)
                {
                    return Fallback.Recipe;
                }

                // TODO: There's a better way to provide translations than this
                return SolarEcs.Recipe.Empty<NameModel>();
            }
        }
    }
}
