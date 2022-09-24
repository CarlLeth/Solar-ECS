using Solar.Common.Identification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.Globalization.Translations
{
    public class CultureDependentNameSystem : INameSystem
    {
        private IQueryPlan<CultureDependentName> Names;
        private ITextCulture CurrentCulture;

        public CultureDependentNameSystem(IQueryPlan<CultureDependentName> names, ITextCulture currentCulture)
        {
            this.Names = names;
            this.CurrentCulture = currentCulture;
        }

        public IQueryPlan<NameModel> Query
        {
            get
            {
                if (!CurrentCulture.IsAvailable)
                {
                    return QueryPlan.Empty<NameModel>();
                }

                return Names
                    .Where(o => o.Model.Culture == CurrentCulture.Id)
                    .ShiftKey(o => o.Model.Entity)
                    .Select(o => new NameModel(o.Name));
            }
        }


        public IRecipe<NameModel> Recipe
        {
            get { return Solar.Recipe.Empty<NameModel>(); }
        }
    }
}
