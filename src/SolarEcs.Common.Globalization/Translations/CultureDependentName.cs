using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Globalization.Translations
{
    public class CultureDependentName
    {
        public Guid Entity { get; private set; }
        public Guid Culture { get; private set; }
        public string Name { get; private set; }

        public CultureDependentName(Guid entity, Guid culture, string name)
        {
            this.Entity = entity;
            this.Culture = culture;
            this.Name = name;
        }

        private CultureDependentName() { }
    }
}
