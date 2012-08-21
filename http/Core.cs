using System;
using System.IO;
using System.Net;
using System.Text;

namespace http
{
    public class RunResult
    {
        public string ErrorMessage { get; set; }
        public int ExitCode { get; set; }
        public string ResponseBody { get; set; }
        public int ResponseCode { get; set; }
    }

    public  static class Core
    {

        public static RunResult Run(string[] args)
        {
            var result = new RunResult();
            var options = Parser.ParseArgs(args);

            HttpWebResponse response = null;

            try
            {
                response = Client.GetResponse(options);

                

                if (options.CheckStatus)
                {
                    result.ExitCode = GetExitStatus(response.StatusCode, options.AllowRedirects);
                }

                var receivedStream = response.GetResponseStream();

                if (receivedStream != null && receivedStream.CanRead)
                {
                    var encode = Encoding.GetEncoding(!string.IsNullOrEmpty(response.ContentEncoding) ? response.ContentEncoding : "utf-8");
                    var readStream = new StreamReader(receivedStream, encode);
                    result.ResponseBody = readStream.ReadToEnd();
                    readStream.Close();
                }
                result.ResponseCode = (int)response.StatusCode;

            }
            catch (WebException web)
            {
                if (web.Response != null)
                {
                    response = (HttpWebResponse)web.Response;
                    result.ResponseCode = (int)response.StatusCode;
                    result.ExitCode = GetExitStatus(response.StatusCode, options.AllowRedirects);
                    result.ErrorMessage = web.Message;
                }
                else
                {
                    result.ExitCode = Consts.EXIT.ERROR;
                    result.ErrorMessage = web.Status + " " + web.Message;
                }
                
            }
            catch (Exception ex)
            {
                result.ExitCode = Consts.EXIT.ERROR;
                result.ErrorMessage = ex.Message;
            }


            Output.Write(options, result, response);
            
            if (response != null)
                response.Close();
            return result;
        }

        private static int GetExitStatus(HttpStatusCode statusCode, bool allowRedirects = false)
        {
            var code = (int) statusCode;

            if (code >= 300 && code <= 399 && !allowRedirects)
            {
                return Consts.EXIT.ERROR_HTTP_3XX;
            }
            if (code >= 400 && code <= 499)
            {
                return Consts.EXIT.ERROR_HTTP_4XX;
            }
            if (code >= 500 && code <= 599)
            {
                return Consts.EXIT.ERROR_HTTP_5XX;
            }
            return Consts.EXIT.OK;
        }
    }
}
