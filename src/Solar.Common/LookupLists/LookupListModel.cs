using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.LookupLists
{
    public class LookupListModel
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public double? Ordinal { get; private set; }

        public LookupListModel(string name, string description, double? ordinal)
        {
            this.Name = name;
            this.Description = description;
            this.Ordinal = ordinal;
        }

        private LookupListModel() { }
    }
}
