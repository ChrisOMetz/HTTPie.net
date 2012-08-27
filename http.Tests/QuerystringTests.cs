using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace http.Tests
{
    [TestClass]
    public class QuerystringTests : SimpleTestsBase
    {
        [TestMethod]
        public void Test_Querystring_Params_in_Url()
        {
            const string path = "/get?a=1&b=2";
            var url = httpbin(path);
            var result = http(new[] { "GET", url });
            Assert.AreEqual(Consts.EXIT.OK, result.ExitCode, "invalid exit code");

            Assert.IsTrue(result.ResponseBody.Contains("\"url\": \"" + url + "\""));
        }

        [TestMethod]
        public void Test_Querystring_Params_Items()
        {
            const string path = "/get?a=1&b=2";
            var url = httpbin(path);
            var result = http(new[] { "GET", httpbin("/get"), "a==1", "b==2" });
            Assert.AreEqual(Consts.EXIT.OK, result.ExitCode, "invalid exit code");

            Assert.IsTrue(result.ResponseBody.Contains("\"url\": \"" + url + "\""));
        }

        [TestMethod]
        public void Test_Querystring_Params_In_Url_And_Items_With_Duplicates()
        {
            var result = http(new[] { "GET", httpbin("/get?a=1&a=1"), "a==1", "a==1", "b==2" });

            const string path = "/get?a=1&a=1&a=1&a=1&b=2";
            var url = httpbin(path);

            Assert.AreEqual(Consts.EXIT.OK, result.ExitCode, "invalid exit code");
            Assert.IsTrue(result.ResponseBody.Contains("\"url\": \"" + url + "\""));
        }
    }
}
