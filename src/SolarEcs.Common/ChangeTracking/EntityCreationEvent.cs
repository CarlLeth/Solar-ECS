using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.ChangeTracking
{
    [Obsolete("Use EntityChangeEvent")]
    public class EntityCreationEvent
    {
        public Guid CreatingAgent { get; private set; }
        public DateTime CreationDate { get; private set; }

        public EntityCreationEvent(Guid creatingAgent, DateTime creationDate)
        {
            this.CreatingAgent = creatingAgent;
            this.CreationDate = creationDate;
        }

        private EntityCreationEvent() { }
    }
}
