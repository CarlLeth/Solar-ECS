using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.Identification
{
    public class TextModel
    {
        public string Text { get; private set; }

        public TextModel(string text)
        {
            this.Text = text;
        }

        private TextModel() { }
    }
}
