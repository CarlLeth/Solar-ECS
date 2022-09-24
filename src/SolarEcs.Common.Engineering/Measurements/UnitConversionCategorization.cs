using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Engineering.Measurements
{
    public class UnitConversionCategorization
    {
        public Guid UnitConversionCategory { get; private set; }

        public UnitConversionCategorization(Guid unitConversionCategory)
        {
            this.UnitConversionCategory = unitConversionCategory;
        }

        private UnitConversionCategorization() { }
    }
}
