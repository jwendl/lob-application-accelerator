using System;
using System.Collections.Generic;
using System.Text;

namespace LobAccelerator.Library.Utils
{
    static class RandomHelper
    {
        static readonly Random random = new Random();

        public static string GetRandomString(int length)
        {
            return string.Concat(GetCharSequence(length));
        }

        private static IEnumerable<char> GetCharSequence(int length)
        {
            for (int i = 0; i < length; i++)
                yield return (char) random.Next(97, 122);
        }
    }
}
