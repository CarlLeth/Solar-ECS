using SolarEcs.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.ChangeTracking
{
    public class AllChangesTrackingSystem : IChangeTrackingSystem
    {
        private IStore<EntityChangeEvent> ChangeEvents;
        private IDataAgent DataAgent;
        private ICurrentDateTime CurrentDateTime;

        public AllChangesTrackingSystem(IStore<EntityChangeEvent> changeEvents, IDataAgent dataAgent, ICurrentDateTime currentDateTime)
        {
            ChangeEvents = changeEvents;
            DataAgent = dataAgent;
            CurrentDateTime = currentDateTime;
        }

        public IQueryPlan<WithChangeEvents<T>> AttachTo<T>(IQueryPlan<T> baseQuery)
        {
            return baseQuery
                .GroupJoin(ChangeEvents.ToQueryPlan(), root => root.Key, change => change.Model.Entity)
                .ResolveJoin((root, changes) => new WithChangeEvents<T>(
                    root.Model,
                    changes.Select(c => new ChangeEventStub(c.Model.ModifyingAgent, c.Model.ModificationDate)).OrderBy(c => c.ModificationDate)
                ));
        }

        public IRecipe<T> AttachTo<T>(IRecipe<T> baseRecipe, Guid? changingAgent = null)
        {
            return baseRecipe.Include(ChangeEvents.ToRecipe(),
                assign: (trans, id, model) => trans.Add(new EntityChangeEvent(id, changingAgent ?? DataAgent.Id, CurrentDateTime.UtcNow)),
                unassign: (trans, id) => trans.Add(new EntityChangeEvent(id, changingAgent ?? DataAgent.Id, CurrentDateTime.UtcNow))
            );
        }

        public IWritePlan<T> AttachTo<T>(IWritePlan<T> baseWritePlan, Guid? changingAgent = null)
        {
            return baseWritePlan.Include(ChangeEvents.ToWritePlan(),
                (script, part) =>
                {
                    foreach (var key in script.AllKeys)
                    {
                        part.Add(new EntityChangeEvent(key, changingAgent ?? DataAgent.Id, CurrentDateTime.UtcNow));
                    }
                }
            );
        }
    }
}
