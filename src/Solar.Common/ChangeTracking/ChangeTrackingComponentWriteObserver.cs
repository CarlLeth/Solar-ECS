using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.ChangeTracking
{
    public class ChangeTrackingComponentWriteObserver : IComponentWriteObserver
    {
        private Lazy<IStore<EntityCreationEvent>> CreationEvents { get; set; }
        private Lazy<IStore<EntityModificationEvent>> ModificationEvents { get; set; }
        private IDataAgent CurrentAgent { get; set; }

        public ChangeTrackingComponentWriteObserver(Lazy<IStore<EntityCreationEvent>> creationEvents, Lazy<IStore<EntityModificationEvent>> modificationEvents,
            IDataAgent currentAgent)
        {
            this.CreationEvents = creationEvents;
            this.ModificationEvents = modificationEvents;
            this.CurrentAgent = currentAgent;
        }

        public void OnComponentAssigned(Guid entity, object assignedComponent)
        {
            if (!IsChangeTrackingComponent(assignedComponent))
            {
                RegisterEntityChange(entity);
            }
        }

        public void OnComponentUnassigned(Guid entity)
        {
            RegisterEntityChange(entity);
        }

        private bool IsChangeTrackingComponent(object component)
        {
            return component is EntityCreationEvent || component is EntityModificationEvent;
        }

        private void RegisterEntityChange(Guid entity)
        {
            if (!CreationEvents.Value.Contains(entity))
            {
                CreationEvents.Value.AssignCommit(entity, new EntityCreationEvent(CurrentAgent.Id, DateTime.Now));
            }

            ModificationEvents.Value.AssignCommit(entity, new EntityModificationEvent(CurrentAgent.Id, DateTime.Now));
        }
    }
}
