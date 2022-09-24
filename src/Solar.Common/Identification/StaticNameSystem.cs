using Solar.Common.Identification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.Identification
{
    public class StaticNameSystem : INameSystem
    {
        public IQueryPlan<NameModel> Query { get; private set; }
        public IRecipe<NameModel> Recipe { get; private set; }

        public StaticNameSystem(IStore<StaticName> names)
        {
            this.Query = names.ToQueryPlan()
                .Select(o => new NameModel(o.Name));

            this.Recipe = names.ToRecipe()
                .Select(staticName => new NameModel(staticName.Name), nameModel => new StaticName(nameModel.Name));
        }
    }
}
