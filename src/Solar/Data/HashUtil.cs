using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.Data
{
    // See https://stackoverflow.com/a/34006336/1362272
    public static class HashUtil
    {
        public static int CombineHashes(IEnumerable<int> hashes)
        {
            unchecked
            {
                int combined = 1009;
                foreach (int h in hashes)
                {
                    combined = (combined * 9176) + h;
                }

                return combined;
            }
        }

        public static int CombineHashes(params int[] hashes)
        {
            return CombineHashes(hashes);
        }

        public static int CombineHashes(params object[] objectsToHash)
        {
            return CombineHashes(objectsToHash.Select(o => o.GetHashCode()));
        }

        public static int CombineHashes(IEnumerable<object> objectsToHash)
        {
            return CombineHashes(objectsToHash.Select(o => o.GetHashCode()));
        }
    }
}
