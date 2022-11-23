using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolarEcs.Common.Hierarchy;

namespace SolarEcs.Tests.NetCore
{
    [TestClass]
    public class HierarchyTests : SolarTestsBase
    {
        [TestMethod]
        public void CanConstructBasicHierarchy()
        {
            Guid food = Guid.NewGuid();
            
            Guid fruit = Guid.NewGuid();
            Guid meat = Guid.NewGuid();

            Guid strawberry = Guid.NewGuid();
            Guid banana = Guid.NewGuid();
            Guid blueberry = Guid.NewGuid();

            Guid beef = Guid.NewGuid();
            Guid pork = Guid.NewGuid();
            Guid lamb = Guid.NewGuid();
            Guid venison = Guid.NewGuid();

            Add(new HierarchyPosition(food, fruit, food, 1));
            Add(new HierarchyPosition(food, meat, food, 2));

            Add(new HierarchyPosition(food, strawberry, fruit, 1));
            Add(new HierarchyPosition(food, banana, fruit, 2));
            Add(new HierarchyPosition(food, blueberry, fruit, 3));

            Add(new HierarchyPosition(food, beef, meat, 10));
            Add(new HierarchyPosition(food, pork, meat, 3));
            Add(new HierarchyPosition(food, lamb, meat, 1));
            Add(new HierarchyPosition(food, venison, meat, 2.7));

            var hierarchySystem = Build<IHierarchyService>();

            var foodTree = hierarchySystem.ConstructHierarchy(food);

            Assert.AreEqual(food, foodTree.Entity);
            Assert.AreEqual(2, foodTree.Children.Count());

            var fruitTree = foodTree.Children.Where(o => o.Entity == fruit).Single();
            Assert.AreEqual(3, fruitTree.Children.Count());
            Assert.AreEqual(0, fruitTree.Children.First().Children.Count());

            var fruitArray = fruitTree.Children.Select(o => o.Entity).ToArray();
            Assert.AreEqual(strawberry, fruitArray[0]);
            Assert.AreEqual(banana, fruitArray[1]);
            Assert.AreEqual(blueberry, fruitArray[2]);

            var meatTree = foodTree.Children.Where(o => o.Entity == meat).Single();
            Assert.AreEqual(4, meatTree.Children.Count());
            Assert.AreEqual(0, meatTree.Children.First().Children.Count());

            var meatArray = meatTree.Children.Select(o => o.Entity).ToArray();
            Assert.AreEqual(lamb, meatArray[0]);
            Assert.AreEqual(venison, meatArray[1]);
            Assert.AreEqual(pork, meatArray[2]);
            Assert.AreEqual(beef, meatArray[3]);
        }
    }
}
