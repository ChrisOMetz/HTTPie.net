using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace http
{
    internal static class Client
    {
        private const string FORM = @"application/x-www-form-urlencoded; charset=utf-8";
        private const string JSON = @"application/json; charset=utf-8";


        public static HttpWebResponse GetResponse(Options args)
        {
            var request = WebRequest.Create(args.Item.Url) as HttpWebRequest;
            request.Method = args.Item.Method;

            if (args.Item.Timeout > 0)
                request.Timeout = args.Item.Timeout;

            request.UserAgent = "HTTPie.net";

            var response = request.GetResponse();
            return response as HttpWebResponse;
        }

        //private static Options GetResponseKwargs(Options args)
        //{
        //    // Send the request and return a request.Response.
        //    dynamic kwargs = new ExpandoObject();

        //    if (args.json == true)
        //    {
        //        if (!args.headers.Contain("Content-Type"))
        //            args.headers.Add("Content-Type", JSON);


        //    }
        //    else if (args.form == true)
        //    {
        //        if (!args.headers.Contain("Content-Type"))
        //            args.headers.Add("Content-Type", FORM);
        //    }

        //    dynamic credentials = null;
        //    if (args.auth != null)
        //    {
        //        credentials = new ExpandoObject();
        //        credentials.basic = args.auth.HTTPBasicAuth;
        //        credentials.digest = args.auth.HTTPDigestAuth;
        //    }

        //    kwargs.prefetch = false;
        //    kwargs.method = args.method.ToLower();
        //    kwargs.url = args.url;
        //    kwargs.headers = args.headers;
        //    kwargs.data = args.data;
        //    kwargs.verify = (args.verify) ? "yes" : "no";
        //    kwargs.timeout = args.timeout;
        //    kwargs.auth = credentials;
        //    kwargs.proxies = args.proxy ?? "";
        //    kwargs.files = args.files;
        //    kwargs.allow_redirects = args.allow_redirects;
        //    kwargs.parameters = args.parameters;
        //    return kwargs;
        //}
    }
}
