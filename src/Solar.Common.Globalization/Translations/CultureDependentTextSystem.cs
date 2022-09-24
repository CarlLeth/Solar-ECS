using Solar.Common.Identification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.Globalization.Translations
{
    public class CultureDependentTextSystem : ITextSystem
    {
        private IQueryPlan<CultureDependentText> TextQuery;
        private ITextCulture CurrentCulture;

        public CultureDependentTextSystem(IQueryPlan<CultureDependentText> textQuery, ITextCulture currentCulture)
        {
            this.TextQuery = textQuery;
            this.CurrentCulture = currentCulture;
        }

        public IQueryPlan<TextModel> Query
        {
            get
            {
                if (!CurrentCulture.IsAvailable)
                {
                    return QueryPlan.Empty<TextModel>();
                }

                return TextQuery
                    .Where(o => o.Model.Culture == CurrentCulture.Id)
                    .ShiftKey(o => o.Model.Entity)
                    .Select(o => new TextModel(o.Text));
            }
        }

        public IRecipe<TextModel> Recipe
        {
            get
            {
                return Solar.Recipe.Empty<TextModel>();
            }
        }
    }
}
