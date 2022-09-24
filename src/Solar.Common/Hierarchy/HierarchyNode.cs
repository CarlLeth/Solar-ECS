using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.Hierarchy
{
    public class HierarchyNode<TModel>
    {
        public Guid Entity { get; private set; }
        public TModel Model { get; private set; }
        public double Ordinal { get; private set; }
        public IEnumerable<HierarchyNode<TModel>> Children { get; private set; }

        public HierarchyNode(Guid entity, TModel model, double ordinal, IEnumerable<HierarchyNode<TModel>> children)
        {
            this.Entity = entity;
            this.Model = model;
            this.Ordinal = ordinal;
            this.Children = children;
        }

        private HierarchyNode() { }
    }
}
