using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using http.Tests.Helpers;

namespace http.Tests
{
    [TestClass]
    public class BinaryRequestDataTests : SimpleTestsBase
    {
        [TestMethod]
        public void Test_binary_file_path()
        {
            var result = http(new[] { "--print=B", "POST", httpbin("/post"), "@" + BIN_FILE_PATH_ARG });
            
            var s = Encoding.UTF8.GetString(BIN_FILE_CONTENT);
            Assert.IsTrue(result.ResponseBody.Contains(s), "no file content");
        }

        [TestMethod]
        public void Test_binary_file_form()
        {
            var result = http(new[] { "--print=B", "--form", "POST", httpbin("/post"), "test@" + BIN_FILE_PATH_ARG });
            
            var s = Encoding.UTF8.GetString(BIN_FILE_CONTENT);
            Assert.IsTrue(result.ResponseBody.Contains(s), "no file content");
        }
    }
}
