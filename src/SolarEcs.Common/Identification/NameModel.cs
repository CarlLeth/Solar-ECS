using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Identification
{
    public class NameModel
    {
        public string Name { get; private set; }

        public NameModel(string name)
        {
            this.Name = name;
        }

        private NameModel() { }
    }
}
