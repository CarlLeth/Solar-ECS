using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Identification
{
    public class StaticText
    {
        public string Text { get; private set; }

        public StaticText(string text)
        {
            this.Text = text;
        }

        private StaticText() { }
    }
}
