using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.Lists
{
    public class OrderedListMembership
    {
        public Guid Entity { get; private set; }
        public Guid List { get; private set; }
        public double Ordinal { get; private set; }

        public OrderedListMembership(Guid entity, Guid list, double ordinal)
        {
            this.Entity = entity;
            this.List = list;
            this.Ordinal = ordinal;
        }

        private OrderedListMembership() { }
    }
}
