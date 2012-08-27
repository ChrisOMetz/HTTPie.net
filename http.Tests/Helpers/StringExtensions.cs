using System;

namespace http.Tests.Helpers
{
    public static class StringExtensions
    {
        public static int ContainsCount(this string source, string searchTerm)
        {
            int count = 0;
            int n = 0;

            while ((n = source.IndexOf(searchTerm, n, StringComparison.InvariantCulture)) != -1)
            {
                n++;
                count++;
            }

            return count;
        }
    }
}
