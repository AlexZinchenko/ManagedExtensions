using ManagedExtensions.Core.Out;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagedExtensions.Core.Extensions
{
    public static class StringExtensions
    {
        public static string InFixedSpace(this string str, int fixedSpace, Align align)
        {
            var sign = align == Align.Right ? 1 : -1;

            return string.Format("{0, " + sign * fixedSpace + "}", str);
        }

        public static string SubstringWithEnd(this string str, int startIndex, int endIndex)
        {
            return str.Substring(startIndex, endIndex - startIndex + 1);
        }

        public static List<string> Split(this string str, List<int> positions)
        {
            if (!positions.Any())
                return new List<string> { str };

            positions.Sort();

            if (positions.Max() >= str.Length || positions.Min() < 0)
                throw new ArgumentException(nameof(positions));

            var subStrs = new List<string>(positions.Count + 1);

            positions.Insert(0, -1);
            positions.Add(str.Length);

            for (int i = 1; i < positions.Count; i++)
            {
                subStrs.Add(str.SubstringWithEnd(positions[i-1] + 1, positions[i] - 1));
            }
            return subStrs;
        }
    }
}
