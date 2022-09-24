using SolarEcs.Common.Globalization.Translations;
using SolarEcs.Common.Identification;
using SolarEcs.Construction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Globalization
{
    public class GlobalizationComponentPackage : IComponentPackage
    {
        public void Register(IComponentRegistration register)
        {
            register.Component<CultureDependentName>();
            register.Component<CultureDependentText>();
            register.Component<PreferredCulture>();
        }
    }
}
