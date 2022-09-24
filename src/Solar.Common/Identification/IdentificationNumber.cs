using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.Identification
{
    public class IdentificationNumber
    {
        public string Number { get; private set; }

        public IdentificationNumber(string number)
        {
            this.Number = number;
        }

        private IdentificationNumber() { }
    }
}
