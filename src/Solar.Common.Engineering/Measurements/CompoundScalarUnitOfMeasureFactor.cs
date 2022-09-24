using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.Engineering.Measurements
{
    /// <summary>
    /// Represents one factor in a compount scalar unit, along with its exponent.
    /// </summary>
    public class CompoundScalarUnitOfMeasureFactor
    {
        /// <summary>
        /// The compound unit this factor is part of.
        /// </summary>
        public Guid CompoundUnitOfMeasure { get; private set; }

        /// <summary>
        /// The base unit of this factor.
        /// </summary>
        public Guid FactorUnitOfMeasure { get; private set; }

        /// <summary>
        /// The power of this factor, in the numerator when positive and the denominator when negative.
        /// </summary>
        public int Exponent { get; private set; }

        public CompoundScalarUnitOfMeasureFactor(Guid compoundUnitOfMeasure, Guid factorUnitOfMeasure, int exponent)
        {
            this.CompoundUnitOfMeasure = compoundUnitOfMeasure;
            this.FactorUnitOfMeasure = factorUnitOfMeasure;
            this.Exponent = exponent;
        }

        private CompoundScalarUnitOfMeasureFactor() { }

        /// <summary>
        /// Inverts the sign of the exponent, swapping position of this factor between the numerator and denominator.
        /// </summary>
        /// <returns></returns>
        public CompoundScalarUnitOfMeasureFactor Inverted()
        {
            return new CompoundScalarUnitOfMeasureFactor(CompoundUnitOfMeasure, FactorUnitOfMeasure, -Exponent);
        }
    }
}
