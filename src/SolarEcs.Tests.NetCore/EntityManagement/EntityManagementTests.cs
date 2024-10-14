using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolarEcs.Common.Comments;
using SolarEcs.Common.Identification;
using SolarEcs.Common.Lists;
using SolarEcs.EntityManagement;
using SolarEcs.Tests.NetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SolarEcs.Tests.NetCore.EntityManagement
{
    [TestClass]
    public class EntityManagementTests : SolarTestsBase
    {
        public EntityManagementTests()
        {
            //UseEntityFrameworkPersistence();
        }

        [TestMethod]
        public void CanDeleteShallowEntity()
        {
            var entity = Guid.NewGuid();
            Assign(entity, new StaticName("A Static Name"));
            Assign(entity, new StaticText("Some Static Text"));

            Assert.AreEqual("A Static Name", Store<StaticName>().For(entity).Name);
            Assert.AreEqual("Some Static Text", Store<StaticText>().For(entity).Text);

            var deleter = Build<IDeleteEntityAgent>();
            deleter.DeleteEntity(entity);

            Assert.IsFalse(Store<StaticName>().Contains(entity));
            Assert.IsFalse(Store<StaticText>().Contains(entity));
        }

        [TestMethod]
        public void CanDeleteDeepEntityWeb()
        {
            var fruits = Guid.NewGuid();

            Assign(fruits, new StaticName("A List of Fruits"));

            var theBotanicalSociety = Guid.NewGuid();
            Add(new TextComment(fruits, theBotanicalSociety, new DateTime(1974, 5, 1),
                "This official list of fruits commenced by The Botannical Society on May 1st, 1974."));

            var orange = Guid.NewGuid();
            Assign(orange, new StaticName("An Orange"));
            Add(new UnorderedListMembership(orange, fruits));

            var apple = Guid.NewGuid();
            Assign(apple, new StaticName("An Apple"));
            Add(new UnorderedListMembership(apple, fruits));

            var tomato = Guid.NewGuid();
            Assign(tomato, new StaticName("A Tomato"));

            var tomatoListMember = Guid.NewGuid();
            Assign(tomatoListMember, new UnorderedListMembership(tomato, fruits));

            Add(new TextComment(tomatoListMember, theBotanicalSociety, new DateTime(2006, 6, 17),
                "After heated debate, the tomato has been officially added to the list of fruits by The Botannical Society as of June 17th, 2006."));

            var deleter = Build<IDeleteEntityAgent>();
            deleter.DeleteEntity(fruits);

            Assert.AreEqual(0, Store<UnorderedListMembership>().All.Count());
            Assert.AreEqual(0, Store<TextComment>().All.Count());
            Assert.IsFalse(Store<StaticName>().Contains(fruits));

            //All actual fruit entities should have survived the delete.
            Action<string, Guid> expectName = (expectedName, entity) => Assert.AreEqual(expectedName, Store<StaticName>().For(entity).Name);

            expectName("An Orange", orange);
            expectName("An Apple", apple);
            expectName("A Tomato", tomato);
        }

        [TestMethod]
        public void CanCreateDateTimeInQueries()
        {
            var apple = Guid.NewGuid();
            Assign(apple, new StaticName("An Apple"));

            var controlQuery = Store<StaticName>().ToQueryPlan().ImmaterialQuery
                .Select(o => new DateTime(2021, 7, 26));

            var desiredSql = controlQuery.ToString();

            var controlWorks = controlQuery.ToList();

        }
    }
}
