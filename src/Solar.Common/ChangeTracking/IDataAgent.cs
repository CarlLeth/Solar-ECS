using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.ChangeTracking
{
    public interface IDataAgent
    {
        Guid Id { get; }
    }
}
