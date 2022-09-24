using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Lists
{
    public class UnorderedListMembership
    {
        public Guid Entity { get; private set; }
        public Guid List { get; private set; }

        public UnorderedListMembership(Guid entity, Guid list)
        {
            this.Entity = entity;
            this.List = list;
        }

        private UnorderedListMembership() { }
    }
}
