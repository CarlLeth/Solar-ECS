using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Engineering.Measurements
{
    public interface IUnitConversionSystem
    {
        IQueryPlan<IUnitConversionStrategy> Strategies { get; }
    }

    public interface IUnitConversionStrategy
    {
        /// <summary>
        /// Convert the given value, in the represented units, to a normalized value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        double Normalize(double value);

        /// <summary>
        /// Convert the given normalized value to a value in the represented units.
        /// </summary>
        /// <param name="normalValue"></param>
        /// <returns></returns>
        double Denormalize(double normalValue);

        /// <summary>
        /// Scale the given value by this unit according to the given exponent.
        /// Exponent may be negative. Implementations should ignore translational components.
        /// For use in scalar calculations involving multiple units of measure.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="exponent"></param>
        /// <returns></returns>
        double Scale(double value, double exponent);
    }
}
