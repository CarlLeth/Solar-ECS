using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.Globalization.Translations
{
    public class PreferredCulture
    {
        public Guid Culture { get; private set; }

        public PreferredCulture(Guid culture)
        {
            this.Culture = culture; 
        }

        private PreferredCulture() { }
    }
}
