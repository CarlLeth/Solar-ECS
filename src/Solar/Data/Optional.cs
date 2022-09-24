using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Solar
{
    public static class Optional
    {
        public static Optional<T> From<T>(T value)
        {
            return new Optional<T>(Enumerable.Repeat(value, 1));
        }

        public static Optional<T> Empty<T>()
        {
            return new Optional<T>(Enumerable.Empty<T>());
        }
    }

    public class Optional<T>
    {
        public IEnumerable<T> ValueOrEmpty { get; internal set; }

        public Optional(IEnumerable<T> valueOrEmpty)
        {
            this.ValueOrEmpty = valueOrEmpty;
        }

        internal Optional() { }

        private bool HasValue
        {
            get
            {
                return ValueOrEmpty.Any();
            }
        }

        public TValue Get<TValue>(Func<T, TValue> getter)
        {
            return ValueOrEmpty.Select(getter).FirstOrDefault();
        }

        public TValue Get<TValue>(Func<T, TValue> getter, TValue defaultIfEmpty)
        {
            return HasValue ? ValueOrEmpty.Select(getter).FirstOrDefault() : defaultIfEmpty;
        }
    }
}
