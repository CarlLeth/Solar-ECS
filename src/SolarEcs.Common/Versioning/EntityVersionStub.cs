using System;
using System.Collections.Generic;
using System.Text;

namespace SolarEcs.Common.Versioning
{
    public class EntityVersionStub
    {
        public int VersionNumber { get; private set; }
        public DateTime? VersionDate { get; private set; }
        public Guid? ModifyingAgent { get; private set; }
        public bool IsDeleted { get; private set; }

        public EntityVersionStub(int versionNumber, DateTime? versionDate, Guid? modifyingAgent, bool isDeleted)
        {
            VersionNumber = versionNumber;
            VersionDate = versionDate;
            ModifyingAgent = modifyingAgent;
            IsDeleted = isDeleted;
        }

        private EntityVersionStub() { }
    }
}
