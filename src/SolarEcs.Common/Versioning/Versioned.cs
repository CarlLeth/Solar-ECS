using System;
using System.Collections.Generic;
using System.Text;

namespace SolarEcs.Common.Versioning
{
    public class Versioned<T>
    {
        public IEnumerable<VersionModel<T>> Versions { get; private set; }

        public Versioned(IEnumerable<VersionModel<T>> versions)
        {
            Versions = versions;
        }

        private Versioned() { }
    }
}
