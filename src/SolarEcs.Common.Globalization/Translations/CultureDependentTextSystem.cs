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
        private IStore<CultureDependentText> Text;
        private StaticTextSystem Fallback;
        private ITextCulture CurrentCulture;

        public CultureDependentTextSystem(IStore<CultureDependentText> text, StaticTextSystem fallback, ITextCulture currentCulture)
        {
            Text = text;
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

                var cultureDependentText = Text.ToQueryPlan()
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
                return Query.StartRecipe()
                    .Include(Text.ToRecipe(),
                        assign: (trans, id, model) =>
                        {
                            var existing = trans.ExistingModels
                                .Where(o => o.Model.Entity == id && o.Model.Culture == CurrentCulture.Id)
                                .ExecuteKeysOnly();

                            var component = new CultureDependentText(id, CurrentCulture.Id, model.Text);
                            trans.Assign(existing.Any() ? existing.First() : Guid.NewGuid(), component);
                        },
                        unassign: (trans, id) => trans.UnassignWhere(o => o.Entity == id && o.Culture == CurrentCulture.Id)
                    );
            }
        }
    }
}
