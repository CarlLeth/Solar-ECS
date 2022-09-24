using SolarEcs.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs
{
    public interface IReductionStrategy<TResult>
    {
        TResult Reduce(IEnumerable<SystemGeneratedResult<TResult>> systemGeneratedResults);
    }

    public class SystemGeneratedResult<TResult>
    {
        public Type SystemType { get; private set; }
        public TResult Result { get; private set; }

        public SystemGeneratedResult(Type systemType, TResult result)
        {
            this.SystemType = systemType;
            this.Result = result;
        }
    }
}
