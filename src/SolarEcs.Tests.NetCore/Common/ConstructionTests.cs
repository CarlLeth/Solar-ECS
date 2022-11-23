using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolarEcs.Common.Identification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Tests.NetCore
{
    [TestClass]
    public class ConstructionTests : SolarTestsBase
    {
        [TestMethod]
        public void CanBuildEnumerableOfComponents()
        {
            Add(new StaticName("A"));
            Add(new StaticName("B"));
            Add(new StaticName("C"));

            var names = Build<IEnumerable<StaticName>>();

            Assert.AreEqual(3, names.Count());
            Assert.IsTrue(names.Select(o => o.Name).Contains("A"));
        }

        [TestMethod]
        public void CanBuildQueryableOfComponents()
        {
            Add(new StaticName("A"));
            Add(new StaticName("B"));
            Add(new StaticName("C"));

            var names = Build<IQueryable<StaticName>>();

            Assert.AreEqual(3, names.Count());
            Assert.IsTrue(names.Select(o => o.Name).Contains("A"));
        }
    }
}
