using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.Identification
{
    public class StaticTextSystem : ITextSystem
    {
        public IQueryPlan<TextModel> Query { get; private set; }
        public IRecipe<TextModel> Recipe { get; private set; }

        public StaticTextSystem(IStore<StaticText> staticText)
        {
            Query = staticText.ToQueryPlan().Select(o => new TextModel(o.Text));
            Recipe = staticText.ToRecipe().Select(o => new TextModel(o.Text), model => new StaticText(model.Text));
        }
    }
}
