using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Identification
{
    public class StaticTextSystem : ITextSystem
    {
        private IStore<StaticText> StaticText;

        public IQueryPlan<TextModel> Query => StaticText.ToQueryPlan().Select(o => new TextModel(o.Text));
        public IRecipe<TextModel> Recipe => StaticText.ToRecipe().Select(o => new TextModel(o.Text), model => new StaticText(model.Text));

        public IWritePlan<TextModel> WritePlan
        {
            get
            {
                return Query.StartWritePlan()
                    .IncludeSimple(StaticText.ToWritePlan(), o => new StaticText(o.Text));
            }
        }

        public StaticTextSystem(IStore<StaticText> staticText)
        {
            this.StaticText = staticText;
        }
    }
}
