using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.Identification
{
    public class CodeName
    {
        public string Name { get; private set; }

        public CodeName(string name)
        {
            this.Name = name;
        }

        private CodeName() { }
    }
}
