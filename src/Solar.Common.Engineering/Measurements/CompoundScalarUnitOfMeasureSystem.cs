using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Engineering.Measurements
{
    public class CompoundScalarUnitConversionSystem : IUnitConversionSystem
    {
        private IQueryPlan<CompoundScalarUnitOfMeasureFactor> Factors;
        private IUnitConverter UnitConverter;

        public CompoundScalarUnitConversionSystem(IQueryPlan<CompoundScalarUnitOfMeasureFactor> factors, IUnitConverter unitConverter)
        {
            this.Factors = factors;
            this.UnitConverter = unitConverter;
        }

        public IQueryPlan<IUnitConversionStrategy> Strategies
        {
            get
            {
                var strategies = Factors
                    .GroupBy(o => o.CompoundUnitOfMeasure)
                    .Materialize()
                    .Select(o => o.ToList())
                    .Select(o => new LambdaUnitConversionStrategy(NormalizeFunc(o), DenormalizeFunc(o), ScaleFunc(o)));

                return strategies;
            }
        }

        private Func<double, double> NormalizeFunc(IEnumerable<CompoundScalarUnitOfMeasureFactor> factors)
        {
            return measuredValue => Scale(measuredValue, factors.Select(o => (o.FactorUnitOfMeasure, (double)o.Exponent)));
        }

        private Func<double, double> DenormalizeFunc(IEnumerable<CompoundScalarUnitOfMeasureFactor> factors)
        {
            return normalValue => Scale(normalValue, factors.Select(o => (o.FactorUnitOfMeasure, -1.0 * o.Exponent)));
        }

        private Func<double, double, double> ScaleFunc(IEnumerable<CompoundScalarUnitOfMeasureFactor> factors)
        {
            return (measuredValue, outerExponent) => Scale(measuredValue, factors.Select(o => (o.FactorUnitOfMeasure, outerExponent * o.Exponent)));
        }

        private double Scale(double value, IEnumerable<(Guid unit, double exponent)> factors)
        {
            double result = value;

            foreach (var factor in factors)
            {
                result = UnitConverter.Scale(result, factor.unit, factor.exponent);
            }

            return result;
        }
    }
}
