using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolarEcs.Common.Globalization.Translations;
using SolarEcs.Common.Identification;
using SolarEcs.Common.Lists;
using SolarEcs.Common.LookupLists;
using SolarEcs.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusic;

namespace SolarEcs.Tests.NetCore
{
    [TestClass]
    public class ListTests : SolarTestsBase
    {
        readonly Culture CurrentCulture = new Culture();

        public ListTests()
        {
            //UseEntityFrameworkPersistence();
        }

        protected override void RegisterDependencies(IRegisterImplementations register)
        {
            register.RegisterType<INameSystem, CultureDependentNameSystem>();
            register.RegisterType<ITextSystem, CultureDependentTextSystem>();
            register.RegisterInstance<ITextCulture>(CurrentCulture);
        }

        [TestMethod]
        public void CanQueryListMemberModels()
        {
            var list = Guid.NewGuid();

            var item1 = Guid.NewGuid();
            var item2 = Guid.NewGuid();
            var item3 = Guid.NewGuid();

            Add(new OrderedListMembership(item3, list, 3));
            Add(new OrderedListMembership(item2, list, 2));
            Add(new UnorderedListMembership(item1, list));

            var plan = Build<IListSystem>().Query
                .Where(o => o.Model.List == list)
                .OrderBy(o => o.Model.Ordinal);

            var listMembers = plan.ExecuteAll().Select(o => o.Model.Entity).ToList();

            Assert.AreEqual(3, listMembers.Count);
            Assert.AreEqual(item1, listMembers[0]);
            Assert.AreEqual(item2, listMembers[1]);
            Assert.AreEqual(item3, listMembers[2]);
        }

        [TestMethod]
        public void CanQueryNamedListMemberModels()
        {
            var list = Guid.NewGuid();

            var item1 = Guid.NewGuid();
            Assign(item1, new StaticName("Item 1"));

            var item2 = Guid.NewGuid();
            Assign(item2, new StaticName("Item 2"));
            
            var item3 = Guid.NewGuid();
            Assign(item3, new StaticName("Item 3"));

            var noNameItem = Guid.NewGuid();

            Add(new OrderedListMembership(noNameItem, list, 4));
            Add(new OrderedListMembership(item3, list, 3));
            Add(new OrderedListMembership(item2, list, 2));
            Add(new UnorderedListMembership(item1, list));

            var plan = Build<ILookupListSystem>().QueryFor(list)
                .OrderBy(o => o.Model.Ordinal);

            var listMembers = plan.ExecuteAll().ToList();

            Assert.AreEqual(4, listMembers.Count);
            Assert.AreEqual("Item 1", listMembers[0].Model.Name);
            Assert.AreEqual("Item 2", listMembers[1].Model.Name);
            Assert.AreEqual("Item 3", listMembers[2].Model.Name);

            //TODO: Find a way to fall back to ID when name is missing
            //Assert.AreEqual(noNameItem.ToString(), listMembers[3].Model.Name);
            Assert.AreEqual("", listMembers[3].Model.Name);
            Assert.AreEqual(noNameItem, listMembers[3].Key);
        }

        [TestMethod]
        public void CanAddLookupListsWithSystem()
        {
            Guid fruits = Guid.NewGuid();

            var recipe = Build<ILookupListSystem>().RecipeFor(fruits);

            var grape = recipe.AddCommit(new LookupListModel("Grape", null, null));
            var orange = recipe.AddCommit(new LookupListModel("Orange", "A very orangey fruit", null));
            var tangerine =  recipe.AddCommit(new LookupListModel("Tangerine", "Pretty much the same thing as an orange.", null));
            var apple = recipe.AddCommit(new LookupListModel("Apple", "Starts with 'A' but has no ordinal, so should be #3 in the list.", null));
            var banana = recipe.AddCommit(new LookupListModel("Banana", null, 1));
            var strawberry = recipe.AddCommit(new LookupListModel("Strawberry", "Should be the second thing in the list", 2));
            recipe.AssignCommit(strawberry, new LookupListModel("Strawberry", "Actually let's change Strawberry's description", 2));


            var list = Build<ILookupListSystem>().QueryFor(fruits).OrderBy(o => o.Model.Ordinal ?? 999).ThenBy(o => o.Model.Name).ExecuteAll().ToList();

            Assert.AreEqual(6, list.Count);

            Assert.AreEqual(banana, list[0].Key);
            Assert.AreEqual(strawberry, list[1].Key);
            Assert.AreEqual(apple, list[2].Key);
            Assert.AreEqual(grape, list[3].Key);
            Assert.AreEqual(orange, list[4].Key);
            Assert.AreEqual(tangerine, list[5].Key);

            Assert.AreEqual("Banana", list[0].Model.Name);
            Assert.IsTrue(String.IsNullOrEmpty(list[0].Model.Description));

            Assert.AreEqual("Actually let's change Strawberry's description", list[1].Model.Description);
        }

        //[TestMethod]
        public void ListSystemEnforcesUniqueListMembership()
        {
            // TODO: It's not clear this is still desirable. Where should this constraint be enforced?

            var item = Guid.NewGuid();
            var list1 = Guid.NewGuid();
            var list2 = Guid.NewGuid();

            var listRecipe = Build<IListSystem>().Recipe;
            Action<ListMembershipModel> add = listMembership => listRecipe.AddCommit(listMembership);

            Func<int> countUnordered = () => Store<UnorderedListMembership>().Components().Where(o => o.Entity == item).Count();
            Func<int> countOrdered = () => Store<OrderedListMembership>().Components().Where(o => o.Entity == item).Count();

            Action<int, int> expectCounts = (expectedUnordered, expectedOrdered) =>
            {
                Assert.AreEqual(expectedUnordered, countUnordered());
                Assert.AreEqual(expectedOrdered, countOrdered());
            };

            expectCounts(0, 0);

            add(new ListMembershipModel(item, list1, null));
            expectCounts(1, 0);

            add(new ListMembershipModel(item, list1, null));
            expectCounts(1, 0);

            add(new ListMembershipModel(item, list1, 3));
            expectCounts(0, 1);
            //Assert.AreEqual(3, Store<OrderedListMember>().For(item, list)

            add(new ListMembershipModel(item, list1, 16));
            expectCounts(0, 1);

            add(new ListMembershipModel(item, list2, null));
            expectCounts(1, 1);

            add(new ListMembershipModel(item, list1, null));
            expectCounts(2, 0);

            add(new ListMembershipModel(item, list2, null));
            expectCounts(2, 0);

            add(new ListMembershipModel(item, list2, -237.4));
            expectCounts(1, 1);
        }

        [TestMethod]
        public void CanUseEntityQueryForTranslatedList()
        {
            CheckListTranslations(GenerateListFromQuery);
        }

        private IList<LookupListModel> GenerateListFromQuery(Guid letterList)
        {
            var system = Build<ILookupListSystem>();
            var query = system.QueryFor(letterList).OrderBy(o => o.Model.Ordinal);

            var list = query.ExecuteAll().Models();
            return list.ToList();
        }

        private void CheckListTranslations(Func<Guid, IList<LookupListModel>> generateList)
        {
            var letters = Guid.NewGuid();

            Action<string, string, string, string, string> checkLetterNames = (name0, name1, name2, name3, name4) =>
            {
                var list = generateList(letters);
                Assert.AreEqual(5, list.Count());
                Assert.AreEqual(name0, list[0].Name);
                Assert.AreEqual(name1, list[1].Name);
                Assert.AreEqual(name2, list[2].Name);
                Assert.AreEqual(name3, list[3].Name);
                Assert.AreEqual(name4, list[4].Name);
            };

            var alpha = Guid.NewGuid();
            var beta = Guid.NewGuid();
            var gamma = Guid.NewGuid();
            var delta = Guid.NewGuid();
            var epsilon = Guid.NewGuid();

            var english = Guid.NewGuid();
            var greek = Guid.NewGuid();

            Add(new OrderedListMembership(alpha, letters, 1));
            Add(new OrderedListMembership(beta, letters, 2));
            Add(new OrderedListMembership(gamma, letters, 3));
            Add(new OrderedListMembership(delta, letters, 4));
            Add(new OrderedListMembership(epsilon, letters, 5));

            Add(new CultureDependentName(alpha, greek, "Alpha"));
            Add(new CultureDependentName(alpha, english, "A"));

            Add(new CultureDependentName(beta, greek, "Beta"));
            Add(new CultureDependentName(beta, english, "Bee"));

            Assign(gamma, new StaticName("Gamma"));

            Assign(delta, new StaticName("Delta"));
            Add(new CultureDependentName(delta, english, "Dee"));

            Add(new CultureDependentName(epsilon, greek, "Epsilon"));

            //Test 1: Greek.  Every letter has either a static name (in greek) or a greek translation.
            CurrentCulture.Id = greek;
            checkLetterNames("Alpha", "Beta", "Gamma", "Delta", "Epsilon");

            //Test 2: English.
            CurrentCulture.Id = english;
            checkLetterNames(
                "A", //English translation for alpha is "A"
                "Bee", //English translation for beta is "Bee"
                "Gamma", //English has no translation for gamma, so it should fall back to the static name.
                "Dee", //English translation for delta is "Dee"
                "" //No English translation for Epsilon, and no static name to fall back to, so its list name should be empty
            );

            //Test 3: unknown culture
            CurrentCulture.Id = Guid.NewGuid();

            checkLetterNames(
                "", //Alpha has no static name and no translations for this culture, so its name in the list should be its Id.
                "",  //Beta has no static name and no translations for this culture, so its name in the list should be its Id.
                "Gamma", //No translated name found, so it should fall back to the static name.
                "Delta", //No translation found for this culture; fall back to static name.
                "" //Epsilon has no static name and no translations for this culture, so its name should be its Id.
            );

            //TODO: Find a way to fall back to the entity ID when no name exists
            //checkLetterNames(
            //    alpha.ToString(), //Alpha has no static name and no translations for this culture, so its name in the list should be its Id.
            //    beta.ToString(),  //Beta has no static name and no translations for this culture, so its name in the list should be its Id.
            //    "Gamma", //No translated name found, so it should fall back to the static name.
            //    "Delta", //No translation found for this culture; fall back to static name.
            //    epsilon.ToString() //Epsilon has no static name and no translations for this culture, so its name should be its Id.
            //);
        }

        private class Culture : ITextCulture
        {
            public Guid Id { get; set; }
            public bool IsAvailable
            {
                get { return true; }
            }
        }

    }
}
