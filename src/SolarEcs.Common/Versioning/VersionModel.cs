using System;
using System.Collections.Generic;
using System.Text;

namespace SolarEcs.Common.Versioning
{
    public class VersionModel<T>
    {
        public Optional<T> Model { get; private set; }
        public EntityVersionStub Version { get; private set; }

        public VersionModel(Optional<T> model, EntityVersionStub version)
        {
            Model = model;
            Version = version;
        }

        private VersionModel() { }
    }
}
