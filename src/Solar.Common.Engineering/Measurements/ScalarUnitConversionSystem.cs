using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Engineering.Measurements
{
    public class ScalarUnitConversionSystem : IUnitConversionSystem
    {
        public IQueryPlan<IUnitConversionStrategy> Strategies { get; private set; }

        public ScalarUnitConversionSystem(IQueryPlan<ScalarUnitOfMeasure> units)
        {
            this.Strategies = units
                .Materialize()
                .Select(unit => new LambdaUnitConversionStrategy(NormalizeFunc(unit), DenormalizeFunc(unit), ScaleFunc(unit)));
        }

        private Func<double, double> NormalizeFunc(ScalarUnitOfMeasure unit)
        {
            return measuredValue => measuredValue * unit.ConversionFactor;
        }

        private Func<double, double> DenormalizeFunc(ScalarUnitOfMeasure unit)
        {
            return normalValue => normalValue / unit.ConversionFactor;
        }

        private Func<double, double, double> ScaleFunc(ScalarUnitOfMeasure unit)
        {
            return (measuredValue, exponent) => measuredValue * Math.Pow(unit.ConversionFactor, exponent);
        }
    }
}
