using SolarEcs.Common.Engineering.Measurements;
using SolarEcs.Construction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Engineering
{
    public class EngineeringComponentPackage : IComponentPackage
    {
        public void Register(IComponentRegistration register)
        {
            register.ValueType<Measurement>();
            register.Component<CompoundScalarUnitOfMeasureFactor>();
            register.Component<ReferenceUnitOfMeasure>();
            register.Component<ScalarUnitOfMeasure>();
            register.Component<UnitConversionCategorization>();
        }
    }
}
