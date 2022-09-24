using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Engineering.Measurements
{
    public class ReferenceUnitConversionSystem : IUnitConversionSystem
    {
        public IQueryPlan<IUnitConversionStrategy> Strategies { get; private set; }

        public ReferenceUnitConversionSystem(IQueryPlan<ReferenceUnitOfMeasure> units)
        {
            Strategies = units
                .Materialize()
                .Select(unit => new LambdaUnitConversionStrategy(NormalizeFunc(unit), DenormalizeFunc(unit), ScaleFunc(unit)));
        }

        private Func<double, double> NormalizeFunc(ReferenceUnitOfMeasure unit)
        {
            return measuredValue => (measuredValue - unit.ZeroPoint) * unit.ConversionFactor;
        }

        private Func<double, double> DenormalizeFunc(ReferenceUnitOfMeasure unit)
        {
            return normalValue => normalValue / unit.ConversionFactor + unit.ZeroPoint;
        }

        private Func<double, double, double> ScaleFunc(ReferenceUnitOfMeasure unit)
        {
            // Ignore the zero point since we are only scaling
            // e.g. consider units like "degrees Fahrenheit per second". This is a rate of change in temperature,
            // and 0 deg F / s equals 0 deg C / s, not 32 deg C / s.
            return (measuredValue, exponent) => measuredValue * Math.Pow(unit.ConversionFactor, exponent);
        }
    }
}
