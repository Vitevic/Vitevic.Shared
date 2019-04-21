// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
