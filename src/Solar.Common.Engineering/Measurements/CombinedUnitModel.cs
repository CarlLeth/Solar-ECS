using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Engineering.Measurements
{
    public class CombinedUnitModel
    {
        public Guid ConversionCategory { get; private set; }

        public string Name { get; private set; }
        public string Abbreviation { get; private set; }
        public string Description { get; private set; }

        public double? ConversionFactor { get; private set; }
        public double? ZeroPoint { get; private set; }

        public CombinedUnitModel(Guid conversionCategory, string name, string abbreviation, string description, double? conversionFactor, double? zeroPoint)
        {
            ConversionCategory = conversionCategory;
            Name = name;
            Abbreviation = abbreviation;
            Description = description;
            ConversionFactor = conversionFactor;
            ZeroPoint = zeroPoint;
        }

        private CombinedUnitModel() { }
    }
}
