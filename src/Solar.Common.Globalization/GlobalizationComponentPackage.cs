using Solar.Common.Globalization.Translations;
using Solar.Common.Identification;
using Solar.Ecs.Construction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.Globalization
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
