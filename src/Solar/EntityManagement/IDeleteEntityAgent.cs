using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.EntityManagement
{
    public interface IDeleteEntityAgent
    {
        void DeleteEntity(Guid entity);
    }
}
