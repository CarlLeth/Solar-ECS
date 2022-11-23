using SolarEcs.Common;
using SolarEcs.Common.Engineering;
using SolarEcs.Common.Globalization;
using SolarEcs.Data.EntityFramework;
using SolarEcs.Data.EntityFramework.ComponentTableMappers;
using SolarEcs.TestHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Tests.NetCore
{
    public class SolarTestsBase : DomainTestsBase
    {
        public SolarTestsBase()
        {
            AddComponentPackage(new CommonComponentPackage());
            AddComponentPackage(new EngineeringComponentPackage());
            AddComponentPackage(new GlobalizationComponentPackage());
        }

        protected override SandboxComponentDbContext CreateDbContext(IPersistenceTypeLibrary persistenceTypeLibrary)
        {
            var componentTableMapper = new PrefixComponentTableMapper();
            return new SandboxComponentDbContext("Solar.Tests.TestDataContext", persistenceTypeLibrary, componentTableMapper);
        }
    }
}
