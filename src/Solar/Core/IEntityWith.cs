using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar
{
    public interface IEntityWith<TComponent> : IEntity
    {
        TComponent Component { get; }
    }

    public class EntityWith<TComponent> : IEntityWith<TComponent>
    {
        [Key]
        public Guid Id { get; set; }

        public TComponent Component { get; set; }

        public EntityWith(Guid id, TComponent component)
        {
            this.Id = id;
            this.Component = component;
        }

        public EntityWith()
        {
            this.Id = Guid.NewGuid();
        }
    }
}
