using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolarEcs.Common.ChangeTracking;
using SolarEcs.Common.Identification;
using Fusic;

namespace SolarEcs.Tests.NetCore
{
    [TestClass]
    public class ChangeTrackingTests : SolarTestsBase
    {
        [TestMethod]
        public void CanOperateNormallyWithoutTrackChanges()
        {
            Assert.IsFalse(Store<EntityCreationEvent>().All.Any());
            Assert.IsFalse(Store<EntityModificationEvent>().All.Any());

            var colorado = Guid.NewGuid();

            Assign(colorado, new StaticName("Colorado"));
            Assign(colorado, new StaticText("Obviously the best state"));

            Assert.IsFalse(Store<EntityCreationEvent>().All.Any());
            Assert.IsFalse(Store<EntityModificationEvent>().All.Any());

            Assert.AreEqual("Colorado", Store<StaticName>().For(colorado).Name);
        }


        [TestMethod]
        public void CanTrackChanges()
        {
            BuildStrategies.RegisterInstance<IDataAgent>(new CurrentAgentMan());

            Assert.IsFalse(Store<EntityCreationEvent>().All.Any());
            Assert.IsFalse(Store<EntityModificationEvent>().All.Any());

            var colorado = Guid.NewGuid();

            Assign(colorado, new StaticName("Colorado"));
            Assign(colorado, new StaticText("Obviously the best state"));

            Assert.AreEqual(1, Store<EntityCreationEvent>().All.Count());
            Assert.AreEqual(1, Store<EntityModificationEvent>().All.Count());

            var creation = Store<EntityCreationEvent>().For(colorado);
            var modification = Store<EntityModificationEvent>().For(colorado);

            Assert.AreEqual(CurrentAgentMan.CurrentAgent, creation.CreatingAgent);
            Assert.AreEqual(CurrentAgentMan.CurrentAgent, modification.ModifyingAgent);

            Assert.AreEqual(DateTime.Now.Date, creation.CreationDate.Date);
            Assert.AreEqual(DateTime.Now.Date, modification.ModificationDate.Date);

            Assert.AreEqual("Colorado", Store<StaticName>().For(colorado).Name);
        }

        internal class CurrentAgentMan : IDataAgent
        {
            public static readonly Guid CurrentAgent = Guid.NewGuid();

            public Guid Id
            {
                get
                {
                    return CurrentAgent;
                }
            }
        }
    }
}
