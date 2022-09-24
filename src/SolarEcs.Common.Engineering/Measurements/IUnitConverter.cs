using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Engineering.Measurements
{
    /// <summary>
    /// Supports common unit operations for in-memory calculations.
    /// </summary>
    public interface IUnitConverter
    {
        /// <summary>
        /// Converts a Measurement into a normalized value.
        /// </summary>
        /// <param name="measurement"></param>
        /// <returns></returns>
        double Normalize(Measurement measurement);

        /// <summary>
        /// Converts a normalized value into a Measurement in the specified unit.
        /// </summary>
        /// <param name="normalValue"></param>
        /// <param name="targetUnitOfMeasure"></param>
        /// <returns></returns>
        Measurement Denormalize(double normalValue, Guid targetUnitOfMeasure);

        /// <summary>
        /// Scales a value by a unit of measure raised to an exponent.
        /// Exponent may be negative to represent division.
        /// For use in scalar calculations involving multiple units of measure.
        /// </summary>
        /// <param name="measuredValue"></param>
        /// <param name="byUnitOfMeasure"></param>
        /// <param name="exponent"></param>
        /// <returns></returns>
        double Scale(double measuredValue, Guid byUnitOfMeasure, double exponent);

        /// <summary>
        /// Returns the unit conversion category the given unit belongs to.
        /// </summary>
        /// <param name="unitOfMeasure"></param>
        /// <returns></returns>
        Guid GetConversionCategory(Guid unitOfMeasure);
    }

    public static class IUnitConverterExtensions
    {
        /// <summary>
        /// Converts a measurement into a different unit.
        /// </summary>
        /// <param name="converter"></param>
        /// <param name="measurement"></param>
        /// <param name="targetUnitOfMeasure"></param>
        /// <returns></returns>
        public static Measurement Convert(this IUnitConverter converter, Measurement measurement, Guid targetUnitOfMeasure)
        {
            return converter.Denormalize(converter.Normalize(measurement), targetUnitOfMeasure);
        }

        /// <summary>
        /// Scales a value by a sequence of (unit of measure, exponent) factors.
        /// Exponents may be negative to represent division.
        /// </summary>
        /// <param name="unitConverter"></param>
        /// <param name="measuredValue"></param>
        /// <param name="factors"></param>
        /// <returns></returns>
        public static double Scale(this IUnitConverter unitConverter, double measuredValue, params (Guid unit, double exponent)[] factors)
        {
            double result = measuredValue;

            foreach (var factor in factors)
            {
                result = unitConverter.Scale(result, factor.unit, factor.exponent);
            }

            return result;
        }
    }
}
