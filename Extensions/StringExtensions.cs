// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
