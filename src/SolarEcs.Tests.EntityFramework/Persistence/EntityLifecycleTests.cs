using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolarEcs.Tests.EntityFramework.Infrastructure;
using System;
using System.Linq;

namespace SolarEcs.Tests.EntityFramework.Persistence
{
    /// <summary>
    /// Verifies entity-level operations on top of the EF provider:
    /// SandboxComponentDbContext.PermanentlyDestroyAllData and DeleteEntityAgent.
    /// </summary>
    [TestClass]
    public class EntityLifecycleTests : EfTestsBase
    {
        public EntityLifecycleTests()
        {
            UseSqlPersistence();
        }

        [TestMethod]
        public void PermanentlyDestroyAllDataClearsEveryComponentStore()
        {
            Add(new TextComponent { Text = "One" });
            Add(new TextComponent { Text = "Two" });
            Add(new RichComponent { Text = "Rich", Number = 1 });
            Add(new MoneyAndLabelComponent { Label = "Label", Money = new Money { Currency = "USD", Amount = 1 } });
            Add(new BlobComponent { Data = new byte[] { 1 } });

            Assert.IsTrue(Store<TextComponent>().All.Any());

            Catalog.PermanentlyDestroyAllData();

            Assert.AreEqual(0, Store<TextComponent>().All.Count());
            Assert.AreEqual(0, Store<RichComponent>().All.Count());
            Assert.AreEqual(0, Store<MoneyAndLabelComponent>().All.Count());
            Assert.AreEqual(0, Store<BlobComponent>().All.Count());
        }

        [TestMethod]
        public void CanDeleteShallowEntity()
        {
            var entity = Guid.NewGuid();
            Assign(entity, new TextComponent { Text = "Doomed" });
            Assign(entity, new RichComponent { Text = "Also doomed", Number = 2 });

            Assert.AreEqual("Doomed", Store<TextComponent>().For(entity).Text);
            Assert.AreEqual(2, Store<RichComponent>().For(entity).Number);

            var deleter = Build<SolarEcs.EntityManagement.IDeleteEntityAgent>();
            deleter.DeleteEntity(entity);

            Assert.IsNull(Store<TextComponent>().For(entity));
            Assert.IsNull(Store<RichComponent>().For(entity));
        }

        [TestMethod]
        public void DeleteEntityLeavesUnrelatedEntitiesIntact()
        {
            var doomed = Guid.NewGuid();
            var survivor = Guid.NewGuid();

            Assign(doomed, new TextComponent { Text = "Doomed" });
            Assign(survivor, new TextComponent { Text = "Survivor" });

            var deleter = Build<SolarEcs.EntityManagement.IDeleteEntityAgent>();
            deleter.DeleteEntity(doomed);

            Assert.IsNull(Store<TextComponent>().For(doomed));
            Assert.AreEqual("Survivor", Store<TextComponent>().For(survivor).Text);
        }
    }
}
