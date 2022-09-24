using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Identification
{
    public class StaticName
    {
        public string Name { get; private set; }

        public StaticName(string name)
        {
            this.Name = name;
        }

        private StaticName() { }
    }
}
