using System.Collections.Generic;
using Vitevic.Shared.Extensions;

namespace Vitevic.Shared
{
    public class NormalizedPathComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return x.PathEquals(y);
        }

        public int GetHashCode(string obj)
        {
            return obj.NormalizePath().GetHashCode();
        }
    }
}
