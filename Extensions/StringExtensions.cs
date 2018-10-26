using System;
using System.Collections.Generic;
using System.Linq;

namespace Vitevic.Shared.Extensions
{
    public static class StringExtensions
    {
        public static IEnumerable<string> SplitLines(this string input)
        {
            return SplitLines(input, Environment.NewLine);
        }

        public static IEnumerable<string> SplitLines(this string input, string lineEnding)
        {
            if (string.IsNullOrWhiteSpace(input))
                return Enumerable.Empty<string>();

            string[] lines = input.Split(new[] { lineEnding }, StringSplitOptions.None);
            return lines.Length == 0 ? Enumerable.Empty<string>() : lines;
        }
    }
}
