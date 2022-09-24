using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.EntityManagement
{
    public interface IDeleteEntityAgent
    {
        void DeleteEntity(Guid entity);
    }
}
