using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Globalization.Translations
{
    public class CultureDependentText
    {
        public Guid Entity { get; private set; }
        public Guid Culture { get; private set; }
        public string Text { get; private set; }

        public CultureDependentText(Guid entity, Guid culture, string text)
        {
            this.Entity = entity;
            this.Culture = culture;
            this.Text = text;
        }

        private CultureDependentText() { }
    }
}
