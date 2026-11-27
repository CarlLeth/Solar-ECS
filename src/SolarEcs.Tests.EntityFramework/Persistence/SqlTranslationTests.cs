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
    /// Verifies that Solar's query-plan machinery (Where filters, joins, ResolveJoin
    /// selectors, orderings) translates correctly to real SQL Server through EF6's
    /// LINQ provider. This is the layer most likely to regress during an ORM migration.
    /// </summary>
    [TestClass]
    public class SqlTranslationTests : EfTestsBase
    {
        public SqlTranslationTests()
        {
            UseSqlPersistence();
        }

        private readonly Guid[] TextEntities = new Guid[4];

        [TestInitialize]
        public void Seed()
        {
            TextEntities[0] = Add(new TextComponent { Text = "Apple" });
            TextEntities[1] = Add(new TextComponent { Text = "Banana" });
            TextEntities[2] = Add(new TextComponent { Text = "Apricot" });
            TextEntities[3] = Add(new TextComponent { Text = null });

            Add(new RichComponent { Text = "One", Number = 10, OptionalNumber = 1, IsEnabled = true, Amount = 10.5m });
            Add(new RichComponent { Text = "Two", Number = 20, OptionalNumber = null, IsEnabled = false, Amount = null });
            Add(new RichComponent { Text = "Three", Number = 30, OptionalNumber = 3, IsEnabled = true, Amount = 30.75m });
        }

        [TestMethod]
        public void CanFilterByStringEquality()
        {
            var results = Store<TextComponent>().Components().Where(o => o.Text == "Apple").ToList();
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Apple", results[0].Text);
        }

        [TestMethod]
        public void CanFilterByStringPrefixAndContains()
        {
            var startsWithA = Store<TextComponent>().Components().Where(o => o.Text.StartsWith("Ap")).ToList();
            Assert.AreEqual(2, startsWithA.Count);

            var containsAn = Store<TextComponent>().Components().Where(o => o.Text.Contains("an")).ToList();
            Assert.AreEqual(1, containsAn.Count);
            Assert.AreEqual("Banana", containsAn[0].Text);
        }

        [TestMethod]
        public void CanFilterByNullAndNotNull()
        {
            var nullTexts = Store<TextComponent>().Components().Where(o => o.Text == null).ToList();
            Assert.AreEqual(1, nullTexts.Count);

            var nonNullTexts = Store<TextComponent>().Components().Where(o => o.Text != null).ToList();
            Assert.AreEqual(3, nonNullTexts.Count);
        }

        [TestMethod]
        public void CanFilterByNumericComparison()
        {
            var large = Store<RichComponent>().Components().Where(o => o.Number > 15).ToList();
            Assert.AreEqual(2, large.Count);

            var range = Store<RichComponent>().Components().Where(o => o.Number >= 10 && o.Number <= 20).Select(o => o.Text).ToList();
            CollectionAssert.AreEquivalent(new[] { "One", "Two" }, range);
        }

        [TestMethod]
        public void CanFilterByNullableHasValueAndValue()
        {
            var withOptional = Store<RichComponent>().Components().Where(o => o.OptionalNumber.HasValue).ToList();
            Assert.AreEqual(2, withOptional.Count);

            var optionalValue = Store<RichComponent>().Components().Where(o => o.OptionalNumber.Value == 3).Select(o => o.Text).Single();
            Assert.AreEqual("Three", optionalValue);

            var nullAmount = Store<RichComponent>().Components().Where(o => o.Amount == null).Select(o => o.Text).Single();
            Assert.AreEqual("Two", nullAmount);
        }

        [TestMethod]
        public void CanFilterByBoolean()
        {
            var enabled = Store<RichComponent>().Components().Where(o => o.IsEnabled).Select(o => o.Text).ToList();
            CollectionAssert.AreEquivalent(new[] { "One", "Three" }, enabled);
        }

        [TestMethod]
        public void CanProjectAndCount()
        {
            var count = Store<RichComponent>().Components().Count();
            Assert.AreEqual(3, count);

            var texts = Store<RichComponent>().Components().Select(o => o.Text).OrderBy(o => o).ToList();
            CollectionAssert.AreEqual(new[] { "One", "Three", "Two" }, texts);
        }

        [TestMethod]
        public void CanOrderAndTake()
        {
            var top = Store<RichComponent>().Components().OrderByDescending(o => o.Number).Take(2).Select(o => o.Text).ToList();
            CollectionAssert.AreEqual(new[] { "Three", "Two" }, top);
        }

        [TestMethod]
        public void CanExecuteQueryPlanWithKeyPredicate()
        {
            var banana = TextEntities[1];
            var plan = Store<TextComponent>().ToQueryPlan();
            var matches = plan.Execute(key => key == banana).ToList();

            Assert.AreEqual(1, matches.Count);
            Assert.AreEqual("Banana", matches[0].Model.Text);
        }

        [TestMethod]
        public void CanFilterQueryPlanWithComponentPredicate()
        {
            var matches = Store<TextComponent>().ToQueryPlan()
                .Where(o => o.Model.Text != null && o.Model.Text.StartsWith("A"))
                .ExecuteAll().ToList();

            Assert.AreEqual(2, matches.Count);
        }

        [TestMethod]
        public void CanInnerJoinTwoStores()
        {
            var apple = TextEntities[0];
            var banana = TextEntities[1];

            Assign(apple, new RichComponent { Text = "Apple Rich", Number = 1 });
            Assign(banana, new RichComponent { Text = "Banana Rich", Number = 2 });

            var joined = Store<TextComponent>().ToQueryPlan()
                .EntityJoin(Store<RichComponent>().ToQueryPlan(),
                    (text, rich) => new JoinedModel { Text = text.Text, Number = rich.Number, Flag = text.Text == "Apple" })
                .ExecuteAll().ToList();

            Assert.AreEqual(2, joined.Count);

            var appleRow = joined.Single(o => o.Model.Flag);
            Assert.AreEqual("Apple", appleRow.Model.Text);
            Assert.AreEqual(1, appleRow.Model.Number);

            var bananaRow = joined.Single(o => !o.Model.Flag);
            Assert.AreEqual("Banana", bananaRow.Model.Text);
            Assert.AreEqual(2, bananaRow.Model.Number);

            Assert.IsFalse(joined.Any(o => o.Model.Text == "Apricot" || o.Model.Text == null));
        }

        [TestMethod]
        public void CanLeftJoinWithOptionalGetter()
        {
            var apple = TextEntities[0];
            var banana = TextEntities[1];
            var apricot = TextEntities[2];

            Assign(apple, new RichComponent { Text = "Apple Rich", Number = 1 });
            Assign(banana, new RichComponent { Text = "Banana Rich", Number = 2 });
            Assign(apricot, new RichComponent { Text = "Apricot Rich", Number = 3 });

            // Entities: apple/banana/apricot have RichComponents, the null-text entity does not.
            var joined = Store<TextComponent>().ToQueryPlan()
                .EntityLeftJoin(Store<RichComponent>().ToQueryPlan())
                .ResolveJoin((text, rich) => new JoinedModel
                {
                    Text = text.Model.Text,
                    Number = rich.Get(o => o.Number),
                    Flag = rich.Get(o => o.Text) != null,
                })
                .ExecuteAll().ToList();

            Assert.AreEqual(4, joined.Count);

            Assert.AreEqual(1, joined.Single(o => o.Model.Text == "Apple").Model.Number);
            Assert.AreEqual(2, joined.Single(o => o.Model.Text == "Banana").Model.Number);
            Assert.AreEqual(3, joined.Single(o => o.Model.Text == "Apricot").Model.Number);
            Assert.AreEqual(0, joined.Single(o => o.Model.Text == null).Model.Number);
            Assert.IsFalse(joined.Single(o => o.Model.Text == null).Model.Flag);
        }

        [TestMethod]
        public void CanChainDeepLeftJoinsWithFallbacks()
        {
            var moneyStore = Store<MoneyAndLabelComponent>();
            var apple = TextEntities[0];

            Assign(apple, new RichComponent { Text = "Apple Rich", Number = 1 });
            Assign(apple, new MoneyAndLabelComponent { Label = "Apple Money", Money = new Money { Currency = "USD", Amount = 5 } });

            var joined = Store<TextComponent>().ToQueryPlan()
                .EntityLeftJoin(Store<RichComponent>().ToQueryPlan())
                .EntityLeftJoin(moneyStore.ToQueryPlan())
                .EntityLeftJoin(Store<BlobComponent>().ToQueryPlan())
                .ResolveJoin((text, rich, money, blob) => new DeepJoinModel
                {
                    Text = text.Model.Text,
                    Number = rich.Get(o => o.Number),
                    Label = money.Get(o => o.Label),
                    Currency = money.Get(o => o.Money.Currency),
                    Amount = money.Get(o => o.Money.Amount, 0m),
                    HasBlob = blob.Get(o => o.Data) != null,
                })
                .ExecuteAll().ToList();

            Assert.AreEqual(4, joined.Count);

            var appleRow = joined.Single(o => o.Model.Text == "Apple");
            Assert.AreEqual(1, appleRow.Model.Number);
            Assert.AreEqual("Apple Money", appleRow.Model.Label);
            Assert.AreEqual("USD", appleRow.Model.Currency);
            Assert.AreEqual(5m, appleRow.Model.Amount);

            var plainRow = joined.Single(o => o.Model.Text == null);
            Assert.AreEqual(0, plainRow.Model.Number);
            Assert.IsNull(plainRow.Model.Label);
            Assert.IsNull(plainRow.Model.Currency);
            Assert.AreEqual(0m, plainRow.Model.Amount);
            Assert.IsFalse(plainRow.Model.HasBlob);
        }

        private class JoinedModel
        {
            public string Text { get; set; }
            public double Number { get; set; }
            public bool Flag { get; set; }
        }

        private class DeepJoinModel
        {
            public string Text { get; set; }
            public double Number { get; set; }
            public string Label { get; set; }
            public string Currency { get; set; }
            public decimal Amount { get; set; }
            public bool HasBlob { get; set; }
        }
    }
}
