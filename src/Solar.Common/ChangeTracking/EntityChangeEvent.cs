using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.ChangeTracking
{
    public class EntityChangeEvent
    {
        public Guid Entity { get; private set; }
        public Guid ModifyingAgent { get; private set; }
        public DateTime ModificationDate { get; private set; }

        public EntityChangeEvent(Guid entity, Guid modifyingAgent, DateTime modificationDate)
        {
            Entity = entity;
            ModifyingAgent = modifyingAgent;
            ModificationDate = modificationDate;
        }

        private EntityChangeEvent() { }
    }
}
