using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.Engineering.Measurements
{
    public class ScalarUnitOfMeasure
    {
        public double ConversionFactor { get; private set; }

        public ScalarUnitOfMeasure(double conversionFactor)
        {
            this.ConversionFactor = conversionFactor;
        }

        private ScalarUnitOfMeasure() { }
    }
}
