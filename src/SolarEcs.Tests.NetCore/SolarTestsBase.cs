using Fusic;
using SolarEcs.Common;
using SolarEcs.Common.ChangeTracking;
using SolarEcs.Common.Engineering;
using SolarEcs.Common.Globalization;
using SolarEcs.Common.Identification;
using SolarEcs.Data.EntityFramework;
using SolarEcs.Data.EntityFramework.ComponentTableMappers;
using SolarEcs.TestHelpers;
using SolarEcs.Tests.NetCore.Infrastructure;
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

        protected override void RegisterDependencies(IRegisterImplementations register)
        {
            register.RegisterType<INameSystem, StaticNameSystem>();
            register.RegisterType<ITextSystem, StaticTextSystem>();
            register.RegisterType<IDataAgent, TestDataAgent>();
        }

        protected override SandboxComponentDbContext CreateDbContext(IPersistenceTypeLibrary persistenceTypeLibrary)
        {
            var componentTableMapper = new PrefixComponentTableMapper();
            return new SandboxComponentDbContext("Solar.Tests.TestDataContext", persistenceTypeLibrary, componentTableMapper);
        }
    }
}
