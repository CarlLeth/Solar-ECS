using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Identification
{
    public class Alias
    {
        public Guid Entity { get; private set; }
        public string Name { get; private set; }
        public Guid? AliasType { get; private set; }

        public Alias(Guid entity, string name, Guid? aliasType = null)
        {
            this.Entity = entity;
            this.Name = name;
            this.AliasType = aliasType;
        }

        private Alias() { }
    }
}
