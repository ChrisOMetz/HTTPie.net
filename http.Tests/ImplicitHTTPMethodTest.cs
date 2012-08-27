using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace http.Tests
{
    [TestClass]
    public class ImplicitHTTPMethodTest : SimpleTestsBase
    {
        [TestMethod]
        public void Test_implicit_GET()
        {
            var result = http(new[] { httpbin("/get") });

            Assert.AreEqual(Consts.EXIT.OK, result.ExitCode, "invalid exit code");
        }

        [TestMethod]
        public void Test_implicit_GET_with_headers()
        {
            var result = http(new[] { httpbin("/headers"), "Foo:bar" });
            Assert.AreEqual(Consts.EXIT.OK, result.ExitCode, "invalid exit code");
            Assert.IsTrue(result.ResponseBody.Contains("\"Foo\": \"bar\""));
        }

        [TestMethod]
        public void Test_implicit_POST_json()
        {
            var result = http(new[] { httpbin("/post"), "hello=world" });
            Assert.AreEqual(Consts.EXIT.OK, result.ExitCode, "invalid exit code");
            Assert.IsTrue(result.ResponseBody.Contains("\"hello\": \"world\""));
        }

        [TestMethod]
        public void Test_implicit_POST_form()
        {
            var result = http(new[] { "--form", httpbin("/post"), "foo=bar" });
            Assert.AreEqual(Consts.EXIT.OK, result.ExitCode, "invalid exit code");
            Assert.IsTrue(result.ResponseBody.Contains("\"foo\": \"bar\""));
        }
    }
}
