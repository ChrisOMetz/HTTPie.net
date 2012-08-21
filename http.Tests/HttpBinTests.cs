using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace http.Tests
{
    [TestClass]
    public class HttpBinTests : SimpleTestsBase
    {
        
        [TestMethod]
        public void Test_InvalidUrl()
        {
            var result = http(new[] { "GET", "http://nottobefound.local" });
            Assert.AreEqual(Consts.EXIT.ERROR, result.ExitCode);
        }

        [TestMethod]
        public void Test_NotFound()
        {
            var result = http(new[] { "GET", "http://www.enjoy-dialogs.de/gibtsnet" });
            Assert.AreEqual(Consts.EXIT.ERROR_HTTP_4XX, result.ExitCode);
        }

        [TestMethod]
        public void Test_GET()
        {
            var result = http(new[] {"GET", httpbin("/get")});
            Assert.AreEqual(Consts.EXIT.OK, result.ExitCode);
        }

        [TestMethod]
        public void Test_DELETE()
        {
            var result = http(new[] { "DELETE", httpbin("/delete") });
            Assert.AreEqual(Consts.EXIT.OK, result.ExitCode);
        }

        [TestMethod]
        public void Test_PUT()
        {
            var result = http(new[] { "PUT", httpbin("/put"), "foo=bar" });
            Assert.AreEqual(Consts.EXIT.OK, result.ExitCode);
            Assert.IsTrue(result.ResponseBody.Contains("\"foo\": \"bar\""));
        }

        [TestMethod]
        public void Test_POST_JSON_data()
        {
            var result = http(new[] { "POST", httpbin("/post"), "foo=bar" });
            Assert.AreEqual(Consts.EXIT.OK, result.ExitCode);
            Assert.IsTrue(result.ResponseBody.Contains("\"foo\": \"bar\""));
        }

        [TestMethod]
        public void Test_POST_form()
        {
            var result = http(new[] { "--form", "POST", httpbin("/post"), "foo=bar" });
            Assert.AreEqual(Consts.EXIT.OK, result.ExitCode);
            Assert.IsTrue(result.ResponseBody.Contains("\"foo\": \"bar\""));
        }

        [TestMethod]
        public void Test_POST_form_multiple_values()
        {
            var result = http(new[] {"--form", "POST", httpbin("/post"), "foo=bar", "foo=baz"});
            Assert.AreEqual(Consts.EXIT.OK, result.ExitCode);
            JObject o = JObject.Parse(result.ResponseBody);
            var back = o.SelectToken("form", false)
                .Value<JToken>()
                .ToString(Formatting.Indented);
            Assert.IsNotNull(back);
            Assert.IsTrue(back.Replace(Environment.NewLine, "").Contains("\"foo\": [    \"bar\",    \"baz\"  ]"));
        }

        [TestMethod]
        public void Test_Headers()
        {
            var result = http(new[] { "GET", httpbin("/headers"), "Foo:bar" });
            Assert.AreEqual(Consts.EXIT.OK, result.ExitCode);
            Assert.IsTrue(result.ResponseBody.Contains("\"User-Agent\": \"HTTPie.net\""));
            Assert.IsTrue(result.ResponseBody.Contains("\"Foo\": \"bar\""));
        }

    }
}
