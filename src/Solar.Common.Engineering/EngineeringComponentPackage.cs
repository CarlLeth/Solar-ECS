using Solar.Common.Engineering.Measurements;
using Solar.Ecs.Construction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.Engineering
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
