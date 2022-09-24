using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.Identification
{
    public class Abbreviation
    {
        public string Text { get; private set; }

        public Abbreviation(string text)
        {
            this.Text = text;
        }

        private Abbreviation() { }
    }
}
