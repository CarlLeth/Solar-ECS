using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Construction
{
    public class CompositeComponentPackage : IComponentPackage
    {
        private IEnumerable<IComponentPackage> Packages { get; set; }

        public CompositeComponentPackage(params IComponentPackage[] packages)
        {
            this.Packages = packages;
        }

        public void Register(IComponentRegistration register)
        {
            Packages.ForEach(pkg => register.Package(pkg));
        }
    }
}
