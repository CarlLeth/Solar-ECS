using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Hierarchy
{
    public class HierarchyModel
    {
        public Guid Entity { get; private set; }
        public double Ordinal { get; private set; }
        public IEnumerable<HierarchyModel> Children { get; private set; }

        public HierarchyModel(Guid entity, double ordinal, IEnumerable<HierarchyModel> children)
        {
            this.Entity = entity;
            this.Ordinal = ordinal;
            this.Children = children;
        }

        public IEnumerable<HierarchyModel> GetAllDescendents()
        {
            return Enumerable.Repeat(this, 1).Union(Children.SelectMany(o => o.GetAllDescendents()));
        }

        private HierarchyModel() { }
    }
}
