using SolarEcs.Common.Identification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Globalization.Translations
{
    public class CultureDependentTextSystem : ITextSystem
    {
        private IQueryPlan<CultureDependentText> TextQuery;
        private StaticTextSystem Fallback;
        private ITextCulture CurrentCulture;

        public CultureDependentTextSystem(IQueryPlan<CultureDependentText> textQuery, StaticTextSystem fallback, ITextCulture currentCulture)
        {
            TextQuery = textQuery;
            Fallback = fallback;
            CurrentCulture = currentCulture;
        }

        public IQueryPlan<TextModel> Query
        {
            get
            {
                if (!CurrentCulture.IsAvailable)
                {
                    return Fallback.Query;
                }

                var cultureDependentText = TextQuery
                    .Where(o => o.Model.Culture == CurrentCulture.Id)
                    .ShiftKey(o => o.Model.Entity)
                    .Select(o => new TextModel(o.Text));

                return QueryPlan.UnionInPriorityOrder(
                    cultureDependentText,
                    Fallback.Query
                );
            }
        }

        public IRecipe<TextModel> Recipe
        {
            get
            {
                if (!CurrentCulture.IsAvailable)
                {
                    return Fallback.Recipe;
                }

                // TODO: There's a better way to provide translations.
                return SolarEcs.Recipe.Empty<TextModel>();
            }
        }
    }
}
