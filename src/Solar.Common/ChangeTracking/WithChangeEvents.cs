using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.ChangeTracking
{
    public class WithChangeEvents<T>
    {
        public T Model { get; private set; }
        public IEnumerable<ChangeEventStub> ChangeEvents { get; private set; }

        public WithChangeEvents(T model, IEnumerable<ChangeEventStub> changeEvents)
        {
            Model = model;
            ChangeEvents = changeEvents;
        }

        private WithChangeEvents() { }
    }
}
