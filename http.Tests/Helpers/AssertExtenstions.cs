using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace http.Tests.Helpers
{
    public static class AssertExtenstions 
    {
        public static bool AssertContains(this string source, string searchTerm)
        {
            return source.Contains(searchTerm);
        }
    }
}
