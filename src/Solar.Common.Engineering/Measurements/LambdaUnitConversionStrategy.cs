using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Engineering.Measurements
{
    public class LambdaUnitConversionStrategy : IUnitConversionStrategy
    {
        private Func<double, double> NormalizeFunc;
        private Func<double, double> DenormalizeFunc;
        private Func<double, double, double> ScaleFunc;

        public LambdaUnitConversionStrategy(Func<double, double> normalizeFunc, Func<double, double> denormalizeFunc, Func<double, double, double> scaleFunc)
        {
            this.NormalizeFunc = normalizeFunc;
            this.DenormalizeFunc = denormalizeFunc;
            this.ScaleFunc = scaleFunc;
        }

        public double Normalize(double value)
        {
            return NormalizeFunc(value);
        }

        public double Denormalize(double normalValue)
        {
            return DenormalizeFunc(normalValue);
        }

        public double Scale(double value, double exponent)
        {
            return ScaleFunc(value, exponent);
        }
    }
}
