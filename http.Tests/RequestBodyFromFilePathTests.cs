using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace http.Tests
{
    [TestClass]
    public class RequestBodyFromFilePathTests : SimpleTestsBase
    {
        [TestMethod]
        public void Test_request_body_from_file_by_path()
        {
            var result = http(new[] { "POST", httpbin("/post"), "@" + FILE_PATH_ARG });

            Assert.AreEqual(Consts.EXIT.OK, result.ExitCode, "invalid exit code");
            Assert.IsTrue(result.ResponseBody.Contains(FILE_CONTENT), "no file content");
            Assert.IsTrue(result.ResponseBody.Contains("\"Content-Type\": \"text/plain\""), "invalid content-type");
        }
    }
}
