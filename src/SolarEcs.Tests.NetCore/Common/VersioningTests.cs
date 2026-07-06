using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolarEcs.Common.LookupLists;
using SolarEcs.Common.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Tests.NetCore.Common
{
    [TestClass]
    public class ChangeTrackingTests : SolarTestsBase
    {
        public ChangeTrackingTests()
        {
            //UseEntityFrameworkPersistence();
        }

        private static readonly Guid TestList = new Guid("1ddc722e-1328-4590-9631-f61cccc83f72");

        private IQueryPlan<LookupListModel> Query
        {
            get
            {
                return Build<LookupListSystem>().QueryFor(TestList);
            }
        }

        private IRecipe<LookupListModel> Recipe
        {
            get
            {
                return Build<LookupListSystem>().RecipeFor(TestList);
            }
        }

        [TestMethod]
        public void CanTrackVersions()
        {
            var versioningSystem = Build<IVersioningSystem>();

            var salad = Guid.NewGuid();
            var chicken = Guid.NewGuid();
            var pie = Guid.NewGuid();

            // Simulate records being added before Versioning was enabled in the application
            var unversionedTrans = Recipe.CreateTransaction();

            unversionedTrans.Assign(salad, new LookupListModel("Salad", "Description 1", null));
            unversionedTrans.Assign(chicken, new LookupListModel("Chicken", "", null));

            unversionedTrans.Commit();

            var versionedTrans1 = versioningSystem.LatestRecipe(Recipe).CreateTransaction();

            versionedTrans1.Assign(salad, new LookupListModel("Salad", "Description 2", null));
            versionedTrans1.Assign(pie, new LookupListModel("Pi", "A number?", null));

            versionedTrans1.Commit();

            var versionedTrans2 = versioningSystem.LatestRecipe(Recipe).CreateTransaction();

            versionedTrans2.Assign(pie, new LookupListModel("Pie", "A food.", null));
            versionedTrans2.Unassign(chicken);

            versionedTrans2.Commit();

            var latest = versioningSystem.LatestQuery(Query).ExecuteToDictionary();

            Assert.AreEqual(2, latest.Count);
            Assert.AreEqual("Salad", latest[salad].Name);
            Assert.AreEqual("Description 2", latest[salad].Description);
            Assert.AreEqual("Pie", latest[pie].Name);
            Assert.IsFalse(latest.ContainsKey(chicken));

            var withVersions = versioningSystem.AttachVersions(Query).ExecuteToDictionary();

            Assert.AreEqual(2, withVersions.Count);
            Assert.AreEqual("Pie", withVersions[pie].Model.Name);
            Assert.AreEqual(2, withVersions[pie].Versions.Count());
            Assert.IsFalse(withVersions.ContainsKey(chicken));

            var allVersions = versioningSystem.VersionsOf(Query).ExecuteToDictionary();

            Assert.AreEqual(3, allVersions.Count);
            Assert.IsTrue(allVersions[chicken].Versions.First().Version.IsDeleted);
        }
    }
}
