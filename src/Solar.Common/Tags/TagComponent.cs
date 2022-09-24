using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.Categorization
{
    public class TagComponent
    {
        public Guid Entity { get; private set; }
        public Guid Tag { get; private set; }

        public TagComponent(Guid entity, Guid tag)
        {
            this.Entity = entity;
            this.Tag = tag;
        }
    }
}
