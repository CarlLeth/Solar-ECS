using SolarEcs.Common.Identification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Identification
{
    public class StaticNameSystem : INameSystem
    {
        private readonly IStore<StaticName> Names;

        public StaticNameSystem(IStore<StaticName> names)
        {
            Names = names;
        }

        public IQueryPlan<NameModel> Query
        {
            get
            {
                return Names.ToQueryPlan().Select(o => new NameModel(o.Name));
            }
        }

        public IRecipe<NameModel> Recipe
        {
            get
            {
                return Names.ToRecipe()
                    .Select(staticName => new NameModel(staticName.Name), nameModel => new StaticName(nameModel.Name));
            }
        }

        public IWritePlan<NameModel> WritePlan
        {
            get
            {
                return Query.StartWritePlan()
                    .IncludeSimple(Names.ToWritePlan(), nameModel => new StaticName(nameModel.Name));
            }
        }
    }
}
