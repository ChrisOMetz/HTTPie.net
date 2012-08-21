using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace http.Tests
{
    [TestClass]
    public class HttpBinTests
    {
        private const string HTTPBIN_URL = "http://httpbin.org";



        [TestMethod]
        public void Test_InvalidUrl()
        {
            var result = http(new[] { "GET", "http://nottobefound.local" });
            Assert.AreEqual(Consts.EXIT.ERROR_HTTP_4XX, result.StatusCode);
        }

        [TestMethod]
        public void Test_NotFound()
        {
            var result = http(new[] { "GET", "http://www.enjoy-dialogs.de/gibtsnet" });
            Assert.AreEqual(Consts.EXIT.ERROR_HTTP_4XX, result.StatusCode);
        }

        [TestMethod]
        public void Test_GET()
        {
            var result = http(new[] {"GET", httpbin("/get")});
            Assert.AreEqual(Consts.EXIT.OK, result.StatusCode);
        }

        [TestMethod]
        public void Test_DELETE()
        {
            var result = http(new[] { "DELETE", httpbin("/delete") });
            Assert.AreEqual(Consts.EXIT.OK, result.StatusCode);
        }

        [TestMethod]
        public void Test_PUT()
        {
            var result = http(new[] { "PUT", httpbin("/put"), "foo=bar" });
            Assert.AreEqual(Consts.EXIT.OK, result.StatusCode);

        }

        [TestMethod]
        public void Test_POST_JSON_data()
        {
            var result = http(new[] { "POST", httpbin("/post") });
            Assert.AreEqual(Consts.EXIT.OK, result.StatusCode, "foo=bar");
        }

        [TestMethod]
        public void Test_POST_form()
        {
            var result = http(new[] { "--form", "POST", httpbin("/post"), "foo=bar" });
            Assert.AreEqual(Consts.EXIT.OK, result.StatusCode, "foo=bar");
        }

        [TestMethod]
        public void Test_POST_form_multiple_values()
        {
            var result = http(new[] { "--form", "POST", httpbin("/post"), "foo=bar", "foo=baz" });
            Assert.AreEqual(Consts.EXIT.OK, result.StatusCode, "foo=bar");
        }

        [TestMethod]
        public void Test_Headers()
        {
            var result = http(new[] { "GET", httpbin("/headers") });
            Assert.AreEqual(Consts.EXIT.OK, result.StatusCode, "foo:bar");
        }



        private static string httpbin(string path)
        {
            return HTTPBIN_URL + path;
        }

        private static RunResult http(string[] args)
        {
            //Invoke with 'args' and 'kwargs'
            var result = new RunResult();

            try
            {
               result = Core.Run(args);
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                result.StatusCode = Consts.EXIT.ERROR;
            }

            return result;
        }
    }
}
