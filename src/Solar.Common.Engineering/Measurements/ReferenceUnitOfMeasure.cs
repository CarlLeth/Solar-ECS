using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.Engineering.Measurements
{
    public class ReferenceUnitOfMeasure
    {
        public double ConversionFactor { get; private set; }
        public double ZeroPoint { get; private set; }

        public ReferenceUnitOfMeasure(double conversionFactor, double zeroPoint)
        {
            this.ConversionFactor = conversionFactor;
            this.ZeroPoint = zeroPoint;
        }

        private ReferenceUnitOfMeasure() { } 
    }
}
