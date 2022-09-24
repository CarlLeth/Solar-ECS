using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Data
{
    public abstract class CompositeKey<TKeyPart1>
    {
        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            var key = obj as CompositeKey<TKeyPart1>;

            return key != null
                && this.GetKeyPart().Equals(key.GetKeyPart());
        }

        public override int GetHashCode()
        {
            return GetKeyPart().GetHashCode();
        }

        protected abstract TKeyPart1 GetKeyPart();
    }

    public abstract class CompositeKey<TKeyPart1, TKeyPart2>
    {
        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            var key = obj as CompositeKey<TKeyPart1, TKeyPart2>;
            if (key == null)
            {
                return false;
            }

            var parts = GetKeyParts();
            var otherParts = key.GetKeyParts();

            return key != null
                && parts.Item1.Equals(otherParts.Item1)
                && parts.Item2.Equals(otherParts.Item2);
        }

        public override int GetHashCode()
        {
            var parts = GetKeyParts();

            return HashUtil.CombineHashes(
                parts.Item1.GetHashCode(),
                parts.Item2.GetHashCode()
            );
        }

        protected abstract (TKeyPart1, TKeyPart2) GetKeyParts();
    }

    public abstract class CompositeKey<TKeyPart1, TKeyPart2, TKeyPart3>
    {
        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            var key = obj as CompositeKey<TKeyPart1, TKeyPart2, TKeyPart3>;
            if (key == null)
            {
                return false;
            }

            var parts = GetKeyParts();
            var otherParts = key.GetKeyParts();

            return key != null
                && parts.Item1.Equals(otherParts.Item1)
                && parts.Item2.Equals(otherParts.Item2)
                && parts.Item3.Equals(otherParts.Item3);
        }

        public override int GetHashCode()
        {
            var parts = GetKeyParts();

            return HashUtil.CombineHashes(
                parts.Item1.GetHashCode(),
                parts.Item2.GetHashCode(),
                parts.Item3.GetHashCode()
            );
        }

        protected abstract (TKeyPart1, TKeyPart2, TKeyPart3) GetKeyParts();
    }

    public abstract class CompositeKey<TKeyPart1, TKeyPart2, TKeyPart3, TKeyPart4>
    {
        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            var key = obj as CompositeKey<TKeyPart1, TKeyPart2, TKeyPart3, TKeyPart4>;
            if (key == null)
            {
                return false;
            }

            var parts = GetKeyParts();
            var otherParts = key.GetKeyParts();

            return key != null
                && parts.Item1.Equals(otherParts.Item1)
                && parts.Item2.Equals(otherParts.Item2)
                && parts.Item3.Equals(otherParts.Item3)
                && parts.Item4.Equals(otherParts.Item4);
        }

        public override int GetHashCode()
        {
            var parts = GetKeyParts();

            return HashUtil.CombineHashes(
                parts.Item1.GetHashCode(),
                parts.Item2.GetHashCode(),
                parts.Item3.GetHashCode(),
                parts.Item4.GetHashCode()
            );
        }

        protected abstract (TKeyPart1, TKeyPart2, TKeyPart3, TKeyPart4) GetKeyParts();
    }
}
