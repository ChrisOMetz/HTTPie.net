using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace http
{
    internal class Client
    {
        private const string FORM = @"application/x-www-form-urlencoded; charset=utf-8";
        private const string JSON = @"application/json; charset=utf-8";


        public HttpWebRequest Request { get; set; }
        public string RequestBody { get; private set; }

        public HttpWebResponse GetResponse(Options args)
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



            this.Request = WebRequest.Create(url) as HttpWebRequest;
            this.Request.Method = args.Item.Method;

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
                        if (key.Equals("User-Agent", StringComparison.OrdinalIgnoreCase))
                        {
                            this.Request.UserAgent = el[1];
                        }
                        else if (key.Equals("Accept", StringComparison.OrdinalIgnoreCase))
                        {
                            this.Request.Accept = el[1];
                            acceptSetExplicit = true;
                        }
                        else if (key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                        {
                            this.Request.ContentType = el[1];
                            contentTypeSetExplicit = true;
                        }
                        else
                        {
                            this.Request.Headers.Add(el[0], el[1]);
                        }
                    }
                }
            }

            // Adding Files
            if (args.Item.Files != null)
            {
                string formDataBoundary = string.Format("----------{0:N}", Guid.NewGuid());
                string contentType = "multipart/form-data; boundary=" + formDataBoundary;
                var postParameters = new Dictionary<string, object>();

                foreach (var fileParam in args.Item.Files)
                {
                    var fileInfo = fileParam.Split('@');

                    if (fileInfo.Length == 2)
                    {
                        byte[] fileContent = null;

                        try
                        {
                            fileContent = File.ReadAllBytes(fileInfo[1]);
                        }
                        catch
                        {
                        }

                        if (fileContent != null)
                        {
                            postParameters.Add(fileInfo[0], new FileParameter(fileContent, Path.GetFileName(fileInfo[1]), GetContentTypeOfFile(Path.GetExtension(fileInfo[1]))));
                        }
                    }
                }

                if (args.Item.Paramters != null)
                {
                    foreach (var paramter in args.Item.Paramters)
                    {
                        var item = paramter.Split('=');
                        if (item.Length == 2)
                            postParameters.Add(item[0], item[1]);
                    }
                }

                byte[] formData = GetMultipartFormData(postParameters, formDataBoundary);

                this.Request.Method = Consts.HTTP_POST;
                this.Request.ContentType = contentType;
                this.Request.ContentLength = formData.Length;

                using (var requstStream = this.Request.GetRequestStream())
                {
                    requstStream.Write(formData, 0, formData.Length);
                    requstStream.Close();
                }

                this.RequestBody = Encoding.UTF8.GetString(formData);

            }
                // Adding Parameters
            else if (this.Request.Method == Consts.HTTP_POST || this.Request.Method == Consts.HTTP_PUT)
            {
                if (args.Item.Paramters != null)
                {
                    if (this.Request.Method == Consts.HTTP_POST)
                    {
                        if (args.UseForm)
                        {
                            if (!contentTypeSetExplicit)
                                this.Request.ContentType = FORM;
                            if (!acceptSetExplicit)
                                this.Request.Accept = "*/*";
                        }
                        else
                        {
                            if (!contentTypeSetExplicit)
                                this.Request.ContentType = JSON;
                            if (!acceptSetExplicit)
                                this.Request.Accept = "application\\json";
                        }
                    }
                    else if (this.Request.Method == Consts.HTTP_PUT)
                    {
                        if (!acceptSetExplicit)
                            this.Request.Accept = "*/*";
                    }
                    else
                    {
                        if (!contentTypeSetExplicit)
                            this.Request.ContentType = JSON;
                        if (!acceptSetExplicit)
                            this.Request.Accept = "application\\json";
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
                            this.Request.ContentLength = postFormData.Length;
                            using (var dataStream = this.Request.GetRequestStream())
                            {
                                dataStream.Write(postFormData, 0, postFormData.Length);
                            }
                            this.RequestBody = Encoding.UTF8.GetString(postFormData);
                        }
                    }
                    else
                    {
                        var tempList = args.Item.Paramters.Select(p => p.Split('=')).ToDictionary<string[], string, object>(el => el[0], el => el[1]);

                        var jsonData = JsonConvert.SerializeObject(tempList);
                        byte[] postJsonData = Encoding.UTF8.GetBytes(jsonData);

                        this.Request.ContentLength = postJsonData.Length;
                        using (var dataStream = this.Request.GetRequestStream())
                        {
                            dataStream.Write(postJsonData, 0, postJsonData.Length);
                        }
                        this.RequestBody = Encoding.UTF8.GetString(postJsonData);
                    }
                }
                else
                {
                    // No Data to send
                    if (args.UseJson)
                    {
                        if (!acceptSetExplicit)
                            this.Request.Accept = "application\\json";
                        if (!contentTypeSetExplicit)
                            this.Request.ContentType = null;
                    }
                    if (args.UseForm)
                    {
                        if (!contentTypeSetExplicit)
                            this.Request.ContentType = FORM;
                    }
                }
            }


            /// Set Cookies
            // TODO
            //this.Request.CookieContainer = new CookieContainer();






            // Setting defaults
            if (string.IsNullOrEmpty(this.Request.UserAgent))
                this.Request.UserAgent = "HTTPie.net/" + typeof(Client).Assembly.GetName().Version.ToString();

            if (string.IsNullOrEmpty(this.Request.Headers[HttpRequestHeader.AcceptEncoding]))
                this.Request.Headers.Add(HttpRequestHeader.AcceptEncoding, "identity, defalte, compress, gzip");

            if (args.Item.Timeout > 0)
                this.Request.Timeout = args.Item.Timeout;


            if (string.IsNullOrEmpty(this.Request.Accept))
            {
                this.Request.Accept = "*/*";
            }



            // Execute the Request
            var response = this.Request.GetResponse();
            return response as HttpWebResponse;
        }

        private static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            var encoding = Encoding.UTF8;
            var formDataStream = new MemoryStream();
            bool needsCLRF = false;

            foreach (var param in postParameters)
            {
                if (needsCLRF)
                    formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

                needsCLRF = true;

                if (param.Value is FileParameter)
                {
                    var fileToUpload = (FileParameter) param.Value;

                    // Add just the first part of this param, since we will write the file data directly to the Stream 
                    string header = $"--{boundary}\r\nContent-Disposition: form-data; name=\"{param.Key}\"; filename=\"{fileToUpload.FileName ?? param.Key}\";{Environment.NewLine}Content-Type: {fileToUpload.ContentType ?? "application/octet-stream"}{Environment.NewLine}{Environment.NewLine}";

                    formDataStream.Write(encoding.GetBytes(header), 0, encoding.GetByteCount(header));

                    // Write the file data directly to the Stream, rather than serializing it to a string. 
                    formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                }
                else
                {
                    string postData = $"--{boundary}{Environment.NewLine}Content-Disposition: form-data; name=\"{param.Key}\"{Environment.NewLine}{Environment.NewLine}{param.Value}";

                    formDataStream.Write(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));
                }
            }

            // Add the end of the this.Request.  Start with a newline 
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));

            // Dump the Stream into a byte[] 
            formDataStream.Position = 0;
            var formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            return formData;
        }

        private static string GetContentTypeOfFile(string fileExtension)
        {
            var contentType = "application/octet-stream";
            fileExtension = fileExtension.ToLower();

            // TODO: Choose right content type
            contentType = "text/html; charset=utf-8";

            //var registryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(fileExtension);
            //if (registryKey != null && registryKey.GetValue("Content Type") != null)
            //    contentType = registryKey.GetValue("Content Type").ToString();

            return contentType;
        }

    }
}
