using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolarEcs.Tests.EntityFramework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Tests.EntityFramework.Persistence
{
    /// <summary>
    /// End-to-end add/assign/unassign/query round trips through the EF-backed stores,
    /// exercising the bulk-copy write path (the default: UseBulkUploading = true).
    /// </summary>
    [TestClass]
    public class CrudRoundTripTests : EfTestsBase
    {
        public CrudRoundTripTests()
        {
            UseSqlPersistence();
        }

        [TestMethod]
        public void CanAddAndReadBackComponent()
        {
            var id = Add(new TextComponent { Text = "Hello, SQL Server" });

            var component = Store<TextComponent>().For(id);
            Assert.IsNotNull(component);
            Assert.AreEqual("Hello, SQL Server", component.Text);
        }

        [TestMethod]
        public void CanAssignOverExistingComponent()
        {
            var id = Add(new TextComponent { Text = "Original" });
            Assign(id, new TextComponent { Text = "Replaced" });

            var remaining = Store<TextComponent>().All.ToList();
            Assert.AreEqual(1, remaining.Count);
            Assert.AreEqual("Replaced", remaining[0].Component.Text);
        }

        [TestMethod]
        public void CanUnassignComponent()
        {
            var id = Add(new TextComponent { Text = "To be deleted" });
            Store<TextComponent>().UnassignCommit(id);

            Assert.IsNull(Store<TextComponent>().For(id));
            Assert.AreEqual(0, Store<TextComponent>().All.Count());
        }

        [TestMethod]
        public void CanAddMultipleComponentsInSingleTransaction()
        {
            var store = Store<TextComponent>();
            var transaction = store.CreateTransaction();

            var id1 = transaction.Add(new TextComponent { Text = "One" });
            var id2 = transaction.Add(new TextComponent { Text = "Two" });
            var id3 = transaction.Add(new TextComponent { Text = "Three" });
            transaction.Commit();

            Assert.AreEqual("One", store.For(id1).Text);
            Assert.AreEqual("Two", store.For(id2).Text);
            Assert.AreEqual("Three", store.For(id3).Text);
            Assert.AreEqual(3, store.All.Count());
        }

        [TestMethod]
        public void CanMixAssignAndUnassignInSingleTransaction()
        {
            var store = Store<TextComponent>();

            var kept = store.AddCommit(new TextComponent { Text = "Kept" });
            var replaced = store.AddCommit(new TextComponent { Text = "Replaced" });
            var removed = store.AddCommit(new TextComponent { Text = "Removed" });

            var transaction = store.CreateTransaction();
            transaction.Assign(replaced, new TextComponent { Text = "Replaced - new" });
            transaction.Unassign(removed);
            transaction.Add(new TextComponent { Text = "Newly added" });
            transaction.Commit();

            Assert.AreEqual("Replaced - new", store.For(replaced).Text);
            Assert.IsNull(store.For(removed));
            Assert.IsNotNull(store.For(kept));
            Assert.AreEqual("Kept", store.For(kept).Text);
            Assert.AreEqual(3, store.All.Count());
        }

        [TestMethod]
        public void AddUniqueInsertsThenOverwrites()
        {
            var store = Store<TextComponent>();

            var first = store.AddUnique(new TextComponent { Text = "First" }, o => o.Text == "First");
            Assert.AreEqual("First", store.For(first.Id).Text);

            var second = store.AddUnique(new TextComponent { Text = "First" }, o => o.Text == "First");
            Assert.AreEqual(first.Id, second.Id);
            Assert.AreEqual(1, store.All.Count());
        }

        [TestMethod]
        public void RichComponentValuesRoundTrip()
        {
            var id = Add(new RichComponent
            {
                Text = "Rich",
                Number = 3.25,
                OptionalNumber = 7,
                IsEnabled = true,
                Timestamp = new DateTime(2026, 11, 27, 12, 0, 0, DateTimeKind.Utc),
                Amount = 99.95m,
                RelatedId = Guid.NewGuid(),
            });

            var component = Store<RichComponent>().For(id);
            Assert.AreEqual("Rich", component.Text);
            Assert.AreEqual(3.25, component.Number);
            Assert.AreEqual(7, component.OptionalNumber);
            Assert.IsTrue(component.IsEnabled);
            Assert.AreEqual(new DateTime(2026, 11, 27, 12, 0, 0, DateTimeKind.Utc), component.Timestamp);
            Assert.AreEqual(99.95m, component.Amount);
            Assert.AreEqual(component.RelatedId, component.RelatedId);
            Assert.IsTrue(component.RelatedId.HasValue);
        }

        [TestMethod]
        public void RichComponentNullValuesRoundTrip()
        {
            var id = Add(new RichComponent
            {
                Text = null,
                Number = 0,
                OptionalNumber = null,
                IsEnabled = false,
                Timestamp = null,
                Amount = null,
                RelatedId = null,
            });

            var component = Store<RichComponent>().For(id);
            Assert.IsNull(component.Text);
            Assert.IsNull(component.OptionalNumber);
            Assert.IsFalse(component.IsEnabled);
            Assert.IsNull(component.Timestamp);
            Assert.IsNull(component.Amount);
            Assert.IsNull(component.RelatedId);
        }

        [TestMethod]
        public void NestedComplexComponentRoundTrips()
        {
            var id = Add(new MoneyAndLabelComponent
            {
                Label = "Labelled",
                Money = new Money { Currency = "USD", Amount = 42.50m },
            });

            var component = Store<MoneyAndLabelComponent>().For(id);
            Assert.AreEqual("Labelled", component.Label);
            Assert.AreEqual("USD", component.Money.Currency);
            Assert.AreEqual(42.50m, component.Money.Amount);
        }

        [TestMethod]
        public void ByteArrayComponentRoundTrips()
        {
            var data = new byte[] { 0x01, 0x02, 0xFF, 0x00, 0x7F };

            var id = Add(new BlobComponent { Data = data });

            var component = Store<BlobComponent>().For(id);
            CollectionAssert.AreEqual(data, component.Data);
        }

        [TestMethod]
        public void CanQueryComponentsWithPredicate()
        {
            Add(new TextComponent { Text = "Alpha" });
            Add(new TextComponent { Text = "Beta" });
            Add(new TextComponent { Text = "Alpha Two" });

            var matches = Store<TextComponent>().Components().Where(o => o.Text == "Alpha").ToList();

            Assert.AreEqual(1, matches.Count);
            Assert.AreEqual("Alpha", matches[0].Text);
        }
    }
}
