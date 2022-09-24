using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarEcs.Data;

namespace SolarEcs.Common.Engineering.Measurements
{
    public class Measurement : IPartiallyCompletable
    {
        public static Measurement Empty()
        {
            return new Measurement(null, null);
        }

        public double? Value { get; private set; }
        public Guid? UnitOfMeasure { get; private set; }

        public Measurement(double? value, Guid? unitOfMeasure)
        {
            this.Value = value;
            this.UnitOfMeasure = unitOfMeasure;
        }

        private Measurement() { }

        public bool IsComplete()
        {
            return Value.HasValue && UnitOfMeasure.HasValue;
        }
    }
}
