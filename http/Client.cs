using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace http
{
    internal static class Client
    {
        private const string FORM = @"application/x-www-form-urlencoded; charset=utf-8";
        private const string JSON = @"application/json; charset=utf-8";


        public static HttpWebResponse GetResponse(Options args)
        {

            var url = args.Item.Url;

            if (args.Item.QueryStringParameters != null)
            {
                string data = args.Item.QueryStringParameters.Aggregate("", (current, paramter) => current + ("&" + paramter));

                if (!string.IsNullOrEmpty(data))
                {
                    if (!url.Contains('?'))
                    {
                        url += "?" + data.Remove(0, 1);
                    }
                    else
                    {
                        url += data;
                    }

                }
            }



            var request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = args.Item.Method;

            // Adding headers
            bool acceptSetExplicit = false;
            bool contentTypeSetExplicit = false;
            if (args.Item.Headers != null)
            {
                foreach (var header in args.Item.Headers)
                {
                    var el = header.Split(':');
                    if (el.Length == 2)
                    {
                        var key = el[0];
                        if (key.Equals("UserAgent", StringComparison.OrdinalIgnoreCase))
                        {
                            request.UserAgent = el[1];
                        } else if (key.Equals("Accept", StringComparison.OrdinalIgnoreCase))
                        {
                            request.Accept = el[1];
                            acceptSetExplicit = true;
                        }
                        else if (key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                        {
                            request.ContentType = el[1];
                            contentTypeSetExplicit = true;
                        }
                        else
                        {
                            request.Headers.Add(el[0], el[1]);
                        }
                    }
                }
            }

            // Adding Parameters
            if (request.Method == Consts.HTTP_POST || request.Method == Consts.HTTP_PUT)
            {
                if (args.Item.Paramters != null)
                {
                    if (request.Method == Consts.HTTP_POST)
                    {
                        if (args.UseForm)
                        {
                            if (!contentTypeSetExplicit)
                            request.ContentType = FORM;
                            if (!acceptSetExplicit)
                            request.Accept = "*/*";
                        }
                        else
                        {
                            if (!contentTypeSetExplicit)
                            request.ContentType = JSON;
                            if (!acceptSetExplicit)
                            request.Accept = "application\\json";
                        }
                    }
                    else if (request.Method == Consts.HTTP_PUT)
                    {
                        if (!acceptSetExplicit)
                            request.Accept = "*/*";
                    }
                    else
                    {
                        if (!contentTypeSetExplicit)
                            request.ContentType = JSON;
                        if (!acceptSetExplicit)
                            request.Accept = "application\\json";
                    }

                    if (args.UseForm)
                    {
                        byte[] postFormData = null;
                        string formData = args.Item.Paramters.Aggregate("", (x, p) => x + ("&" + p));

                        if (!string.IsNullOrEmpty(formData))
                        {
                            formData = formData.Remove(0, 1);
                            postFormData = Encoding.UTF8.GetBytes(formData);
                        }

                        if (postFormData != null)
                        {
                            request.ContentLength = postFormData.Length;
                            using (var dataStream = request.GetRequestStream())
                            {
                                dataStream.Write(postFormData, 0, postFormData.Length);
                            }
                        }
                    }
                    else
                    {
                        var tempList = args.Item.Paramters.Select(p => p.Split('=')).ToDictionary<string[], string, object>(el => el[0], el => el[1]);

                        var jsonData = JsonConvert.SerializeObject(tempList);
                        byte[] postJsonData = Encoding.UTF8.GetBytes(jsonData);

                        if (postJsonData != null)
                        {
                            request.ContentLength = postJsonData.Length;
                            using (var dataStream = request.GetRequestStream())
                            {
                                dataStream.Write(postJsonData, 0, postJsonData.Length);
                            }
                        }
                    }
                }
                else
                {
                    // No Data to send
                    if (args.UseJson)
                    {
                        if (!acceptSetExplicit)
                            request.Accept = "application\\json";
                        if (!contentTypeSetExplicit)
                            request.ContentType = null;
                    }
                    if (args.UseForm)
                    {
                        if (!contentTypeSetExplicit)
                            request.ContentType = FORM;
                    }
                }
            }




            // Setting defaults
            if (string.IsNullOrEmpty(request.UserAgent))
                request.UserAgent = "HTTPie.net";

            if (args.Item.Timeout > 0)
                request.Timeout = args.Item.Timeout;


            if (string.IsNullOrEmpty(request.Accept))
            {
                request.Accept = "*/*";
            }



            // Execute the Request
            var response = request.GetResponse();
            return response as HttpWebResponse;
        }

    }
}
