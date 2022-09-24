using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Construction
{
    public interface IComponentPackage
    {
        void Register(IComponentRegistration register);
    }
}
