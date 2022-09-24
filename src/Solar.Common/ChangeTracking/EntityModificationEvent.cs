using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.ChangeTracking
{
    public class EntityModificationEvent
    {
        public Guid ModifyingAgent { get; private set; }
        public DateTime ModificationDate { get; private set; }

        public EntityModificationEvent(Guid modifyingAgent, DateTime modificationDate)
        {
            this.ModifyingAgent = modifyingAgent;
            this.ModificationDate = modificationDate;
        }

        private EntityModificationEvent() { }
    }
}
