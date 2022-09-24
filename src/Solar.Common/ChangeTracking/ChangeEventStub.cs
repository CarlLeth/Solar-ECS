using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.ChangeTracking
{
    public class ChangeEventStub
    {
        public Guid ModifyingAgent { get; private set; }
        public DateTime ModificationDate { get; private set; }

        public ChangeEventStub(Guid modifyingAgent, DateTime modificationDate)
        {
            ModifyingAgent = modifyingAgent;
            ModificationDate = modificationDate;
        }

        private ChangeEventStub() { }
    }
}
