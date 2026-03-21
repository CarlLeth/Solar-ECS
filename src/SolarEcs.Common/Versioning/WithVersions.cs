using System;
using System.Collections.Generic;
using System.Text;

namespace SolarEcs.Common.Versioning
{
    public class WithVersions<T>
    {
        public T Model { get; private set; }
        public IEnumerable<EntityVersionStub> Versions { get; private set; }

        public WithVersions(T model, IEnumerable<EntityVersionStub> versions)
        {
            Model = model;
            Versions = versions;
        }

        private WithVersions() { }
    }
}
