using System;
using System.Collections.Generic;
using System.Text;

namespace LobAccelerator.Library.Extensions
{
    public static class IEnumerableStringExtensions
    {
        public static string ToStringLines (this IEnumerable<string> stringcollection)
        {
            var sb = new StringBuilder();
            foreach (var str in stringcollection)
            {
                sb.AppendLine(str);
            }

            return sb.ToString();
        }
    }
}
