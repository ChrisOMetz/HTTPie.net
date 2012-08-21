using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace http.Tests
{
    /// <summary>
    /// Test that Accept and Content-Type correctly defaults to JSON, but can still be overridden. The same with Content-Type when --form or -f is used.
    /// </summary>
    [TestClass]
    public class AutoContentTypeAndAcceptHeadersTests : SimpleTestsBase
    {
        [TestMethod]
        public void Test_GET_no_data_no_auto_headers()
        {
            // JSON headers shouldn't be automatically set for POST with no data.
            var result = http(new[] { "GET", httpbin("/headers") });

            Assert.AreEqual(Consts.EXIT.OK, result.ExitCode);
            Assert.IsTrue(result.ResponseBody.Contains("\"Accept\": \"*/*\""));
            Assert.IsFalse(result.ResponseBody.Contains("\"Content-Type\": \"application/json\""));
        }

        [TestMethod]
        public void Test_POST_no_data_no_auto_headers()
        {
            // JSON headers shouldn't be automatically set for POST with no data.
            var result = http(new[] { "POST", httpbin("/post") });

            Assert.AreEqual(Consts.EXIT.OK, result.ExitCode);
            Assert.IsTrue(result.ResponseBody.Contains("\"Accept\": \"*/*\""), "invalid Accept");
            Assert.IsFalse(result.ResponseBody.Contains("\"Content-Type\": \"application/json\""), "invalid Content-Type");
        }

        [TestMethod]
        public void Test_POST_with_data_auto_JSON_headers()
        {
            var result = http(new[] { "POST", httpbin("/post"), "a=b" });

            Assert.AreEqual(Consts.EXIT.OK, result.ExitCode);
            Assert.IsTrue(result.ResponseBody.Contains((@"""Accept"": ""application\\json""")), "invalid Accept");
            Assert.IsTrue(result.ResponseBody.Contains("\"Content-Type\": \"application/json; charset=utf-8\""), "invalid Content-Type");
        }


        //[TestMethod]
        //public void Test_GET_with_data_auto_JSON_headers()
        //{
        //    // JSON headers should automatically be set also for GET with data.
        //    var result = http(new[] { "GET", httpbin("/get"), "a=b" });

        //    Assert.AreEqual(Consts.EXIT.OK, result.ExitCode);
        //    Assert.IsTrue(result.ResponseBody.Contains((@"""Accept"": ""application\\json""")), "invalid Accept");
        //    Assert.IsTrue(result.ResponseBody.Contains("\"Content-Type\": \"application/json; charset=utf-8\""), "invalid Content-Type");
        //}

        [TestMethod]
        public void Test_POST_explicit_JSON_auto_JSON_accept()
        {
            var result = http(new[] { "--json", "POST", httpbin("/post") });

            Assert.AreEqual(Consts.EXIT.OK, result.ExitCode);
            JObject o = JObject.Parse(result.ResponseBody);
            var back = o.SelectToken("headers", false)
                .Value<JToken>()
                .ToString(Formatting.Indented);
            Assert.IsNotNull(back);


            Assert.IsTrue(back.Contains((@"""Accept"": ""application\\json""")), "invalid Accept");
            Assert.IsTrue(back.Contains(@"""Content-Type"": """""), "invalid Content-Type");
        }


        [TestMethod]
        public void Test_explicit_JSON_explicit_headers()
        {
            var result = http(new[] { "--json", "GET", httpbin("/headers"),
            "Accept:application/xml",
            "Content-Type:application/xml"});

            Assert.AreEqual(Consts.EXIT.OK, result.ExitCode);
            Assert.IsTrue(result.ResponseBody.Contains((@"""Accept"": ""application/xml""")), "invalid Accept");
            Assert.IsTrue(result.ResponseBody.Contains("\"Content-Type\": \"application/xml"), "invalid Content-Type");
        }

        [TestMethod]
        public void Test_POST_form_auto_Content_Type()
        {
            var result = http(new[] { "--form", "POST", httpbin("/post") });

            Assert.AreEqual(Consts.EXIT.OK, result.ExitCode);
            Assert.IsTrue(result.ResponseBody.Contains("\"Content-Type\": \"application/x-www-form-urlencoded; charset=utf-8"), "invalid Content-Type");
        }

        [TestMethod]
        public void Test_POST_form_Content_Type_override()
        {
            var result = http(new[] { "--form", "POST", httpbin("/post"), "Content-Type:application/xml" });

            Assert.AreEqual(Consts.EXIT.OK, result.ExitCode);
            Assert.IsTrue(result.ResponseBody.Contains("\"Content-Type\": \"application/xml"), "invalid Content-Type");
        }
    }
}
