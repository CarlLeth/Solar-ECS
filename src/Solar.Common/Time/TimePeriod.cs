using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Time
{
    public class TimePeriod
    {
        public static readonly TimePeriod Always = new TimePeriod(null, null);

        public DateTime? From { get; private set; }
        public DateTime? Until { get; private set; }

        public TimePeriod(DateTime? from, DateTime? until)
        {
            this.From = from;
            this.Until = until;
        }

        private TimePeriod() { }

        public bool Includes(DateTime d)
        {
            return (!From.HasValue || d >= From.Value) && (!Until.HasValue || d < Until.Value);
        }
    }
}
