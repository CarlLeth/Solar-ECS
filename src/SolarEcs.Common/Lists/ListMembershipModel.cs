using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Lists
{
    public class ListMembershipModel
    {
        public Guid Entity { get; private set; }
        public Guid List { get; private set; }
        public double? Ordinal { get; private set; }

        public ListMembershipModel(Guid entity, Guid list, double? ordinal = null)
        {
            this.Entity = entity;
            this.List = list;
            this.Ordinal = ordinal;
        }

        private ListMembershipModel() { }
    }
}
