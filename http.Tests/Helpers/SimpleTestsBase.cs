using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace http.Tests
{
    public abstract class SimpleTestsBase
    {
        private const string HTTPBIN_URL = "http://httpbin.org";

        protected static string httpbin(string path)
        {
            return HTTPBIN_URL + path;
        }

        protected static RunResult http(string[] args)
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
                result.ExitCode = Consts.EXIT.ERROR;
            }

            return result;
        }
    }
}
