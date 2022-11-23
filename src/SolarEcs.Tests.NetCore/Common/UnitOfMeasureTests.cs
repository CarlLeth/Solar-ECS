using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolarEcs.Common.Engineering.Measurements;

namespace SolarEcs.Tests.NetCore
{
    [TestClass]
    public class UnitOfMeasureTests : SolarTestsBase
    {
        public UnitOfMeasureTests()
        {
            //UseEntityFrameworkPersistence();
        }

        [TestMethod]
        public void CanConvertScalarUnits()
        {
            var foot = Add(new ScalarUnitOfMeasure(1));
            var yard = Add(new ScalarUnitOfMeasure(3));
            var mile = Add(new ScalarUnitOfMeasure(5280));
            var inch = Add(new ScalarUnitOfMeasure(1.0 / 12.0));

            var strategies = Build<UnitConverter>().AllStrategies
                .For(foot, yard, mile, inch)
                .ExecuteAll()
                .ToDictionary(o => o.Key, o => o.Model);

            Assert.AreEqual(4, strategies.Count);

            Assert.AreEqual(30, strategies[yard].Normalize(10));
            Assert.AreEqual(0.5, strategies[inch].Normalize(6));
            Assert.AreEqual(1, strategies[mile].Denormalize(5280));
            Assert.AreEqual(100, strategies[foot].Normalize(100));
            Assert.AreEqual(100, strategies[foot].Denormalize(100));
        }

        [TestMethod]
        public void CanConvertReferenceUnits()
        {
            var kelvin = Add(new ScalarUnitOfMeasure(1));
            var celsius = Add(new ReferenceUnitOfMeasure(1, -273.15));
            var fahrenheit = Add(new ReferenceUnitOfMeasure(5.0 / 9.0, -459.67));

            var strategies = Build<UnitConverter>().AllStrategies
                .For(kelvin, celsius, fahrenheit)
                .ExecuteAll()
                .ToDictionary(o => o.Key, o => o.Model);

            Assert.AreEqual(3, strategies.Count);

            Assert.AreEqual(0, strategies[fahrenheit].Normalize(-459.67), 0.000001);
            Assert.AreEqual(0, strategies[celsius].Normalize(-273.15), 0.000001);

            Assert.AreEqual(273.15, strategies[fahrenheit].Normalize(32), 0.000001);
            Assert.AreEqual(273.15, strategies[celsius].Normalize(0), 0.000001);
            Assert.AreEqual(strategies[fahrenheit].Normalize(-40), strategies[celsius].Normalize(-40), 0.000001);
        }

        [TestMethod]
        public void CanConvertCompoundScalarUnits()
        {
            var foot = Add(new ScalarUnitOfMeasure(1));
            var yard = Add(new ScalarUnitOfMeasure(3));

            var second = Add(new ScalarUnitOfMeasure(1));
            var minute = Add(new ScalarUnitOfMeasure(60));

            var cubicYard = Guid.NewGuid();
            Add(new CompoundScalarUnitOfMeasureFactor(cubicYard, yard, 3));

            var cubicYardPerMinute = Guid.NewGuid();
            Add(new CompoundScalarUnitOfMeasureFactor(cubicYardPerMinute, cubicYard, 1));
            Add(new CompoundScalarUnitOfMeasureFactor(cubicYardPerMinute, minute, -1));

            var strategies = Build<UnitConverter>().AllStrategies
                .For(cubicYard, cubicYardPerMinute)
                .ExecuteAll()
                .ToDictionary(o => o.Key, o => o.Model);

            Assert.AreEqual(2, strategies.Count);

            Assert.AreEqual(27, strategies[cubicYard].Normalize(1));
            Assert.AreEqual(2, strategies[cubicYard].Denormalize(54));

            Assert.AreEqual(27.0 / 60.0, strategies[cubicYardPerMinute].Normalize(1));
            Assert.AreEqual(10, strategies[cubicYardPerMinute].Denormalize(270.0 / 60.0));
        }
    }
}
