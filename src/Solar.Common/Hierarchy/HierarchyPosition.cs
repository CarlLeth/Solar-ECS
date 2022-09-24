using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.Hierarchy
{
    public class HierarchyPosition
    {
        public Guid Hierarchy { get; private set; }
        public Guid Entity { get; private set; }
        public Guid Parent { get; private set; }
        public double Ordinal { get; private set; }

        /// <summary>
        /// Indicates that <paramref name="entity"/> belongs in the <paramref name="hierarchy"/>
        /// as the <paramref name="ordinal"/>-th root node.
        /// </summary>
        public HierarchyPosition(Guid hierarchy, Guid entity, double ordinal)
            : this(hierarchy, entity, hierarchy, ordinal)
        {

        }

        /// <summary>
        /// Indicates that <paramref name="entity"/> belongs in the <paramref name="hierarchy"/>
        /// as the <paramref name="ordinal"/>-th child of <paramref name="parent"/>.
        /// </summary>
        public HierarchyPosition(Guid hierarchy, Guid entity, Guid parent, double ordinal)
        {
            this.Hierarchy = hierarchy;
            this.Entity = entity;
            this.Parent = parent;
            this.Ordinal = ordinal;
        }

        private HierarchyPosition() { }
    }
}
