using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vitevic.Shared.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this T item)
        {
            if (item == null)
                yield break;

            yield return item;
        }
    }
}
