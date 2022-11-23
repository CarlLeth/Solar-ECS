using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Tests.NetCore
{
    public class NameDescriptionModel
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        public NameDescriptionModel(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }

        private NameDescriptionModel() { }
    }
}
