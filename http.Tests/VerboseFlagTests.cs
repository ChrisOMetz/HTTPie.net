using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using http.Tests.Helpers;

namespace http.Tests
{
    [TestClass]
    public class VerboseFlagTests : SimpleTestsBase
    {
        [TestMethod]
        public void Test_Verbose()
        {
            var result = http(new[] {"--verbose", "GET", httpbin("/get"), "test-header:__test__"});
            Assert.AreEqual(Consts.EXIT.OK, result.ExitCode, "invalid exit code");
            Assert.IsNotNull(result.OutputMessage);
            Assert.AreEqual(2, result.OutputMessage.ContainsCount("__test__"));
            Assert.IsTrue(result.ResponseBody.Contains("__test__"));
        }

        [TestMethod]
        public void Test_Verbose_form()
        {
            var result = http(new[] {"--verbose", "--form", "POST", httpbin("/post"), "foo=bar", "baz=bar"});
            Assert.AreEqual(Consts.EXIT.OK, result.ExitCode, "invalid exit code");
            result.ResponseBody.AssertContains("foo=bar&baz=bar");
        }

        [TestMethod]
        public void Test_Verbose_json()
        {
            var result = http(new[] {"--verbose", "POST", httpbin("/post"), "foo=bar", "baz=bar"});
            Assert.AreEqual(Consts.EXIT.OK, result.ExitCode, "invalid exit code");
            Assert.IsNotNull(result.OutputMessage);
            Assert.AreEqual(2, result.OutputMessage.ContainsCount("\"foo\":\"bar\""));
            Assert.IsTrue(result.ResponseBody.Contains("\"foo\": \"bar\""));
        }
    }
}
