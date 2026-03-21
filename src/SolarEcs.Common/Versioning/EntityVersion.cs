using System;
using System.Collections.Generic;
using System.Text;

namespace SolarEcs.Common.Versioning
{
    public class EntityVersion
    {
        public Guid PrimaryEntity { get; private set; }
        public int VersionNumber { get; private set; }
        public DateTime? VersionDate { get; private set; }
        public Guid? ModifyingAgent { get; private set; }
        public bool IsDeleted { get; private set; }

        public EntityVersion(Guid primaryEntity, int versionNumber, DateTime? versionDate, Guid? modifyingAgent, bool isDeleted)
        {
            PrimaryEntity = primaryEntity;
            VersionNumber = versionNumber;
            VersionDate = versionDate;
            ModifyingAgent = modifyingAgent;
            IsDeleted = isDeleted;
        }

        private EntityVersion() { }
    }
}
