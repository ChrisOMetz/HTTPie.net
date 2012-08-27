using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace http.Tests
{
    [TestClass]
    public class BinaryResponseDataTests : SimpleTestsBase
    {
        private const string URL = "http://www.google.com/favicon.ico";

        private readonly byte[] _bindata;

        public BinaryResponseDataTests()
        {
            var webClient = new WebClient();
            _bindata = webClient.DownloadData(URL);
        }

        [TestMethod]
        public void Testbinary_suppresses_when_terminal()
        {
            var result = http(new[] { "GET", URL });
            Assert.AreEqual(Consts.EXIT.OK, result.ExitCode, "invalid exit code");

            Assert.IsTrue(result.ResponseBody.Contains(Output.BINARY_SUPPRESSED_NOTICE));
        }

    }
}
