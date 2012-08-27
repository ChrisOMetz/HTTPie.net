using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using http.Tests.Helpers;

namespace http.Tests
{
    [TestClass]
    public class MultipartFormDataFileUploadTests : SimpleTestsBase
    {
        [TestMethod]
        public void Test_non_existent_file_raises_parse_error()
        {
            var result = http(new[] {"--form", "POST", httpbin("/post"), "foo@/__does_not_exist__"});

            Assert.AreEqual(Consts.EXIT.ERROR, result.ExitCode, "invalid exit code");
            Assert.AreEqual("file '/__does_not_exist__' does not exist.", result.ErrorMessage);
        }

        [TestMethod]
        public void Test_upload_ok()
        {
            var result = http(new[] { "--form", "--verbose", "POST", httpbin("/post"), "test-file@" + FILE_PATH_ARG, "foo=bar" });

            Assert.AreEqual(Consts.EXIT.OK, result.ExitCode , "invalid exit code");
            result.ResponseBody.AssertContains(OK);
            Assert.IsTrue(result.OutputMessage.Contains("Content-Disposition: form-data; name=\"foo\""), "no foo parameter");
            Assert.IsTrue(result.OutputMessage.Contains("Content-Disposition: form-data; name=\"test-file\";"), "no test-file parameter");
            Assert.IsTrue(result.OutputMessage.Contains(string.Format("filename=\"{0}\"", Path.GetFileName(FILE_PATH))), "invalid filename");
            Assert.IsTrue(result.OutputMessage.Contains(FILE_CONTENT), "no file content in request");
            Assert.IsTrue(result.ResponseBody.Contains(FILE_CONTENT), "no file content in response");
            Assert.IsTrue(result.ResponseBody.Contains("\"foo\": \"bar\""));
        }
    }
}
