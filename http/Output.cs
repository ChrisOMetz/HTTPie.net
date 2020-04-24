using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Text;

namespace http
{
    public static class Output
    {
        public const string BINARY_SUPPRESSED_NOTICE = @"
+-----------------------------------------+
| NOTE: binary data not shown in terminal |
+-----------------------------------------+";


        internal static string Write(Options options, HttpWebRequest request, string requestBody)
        {
            var output = new StringBuilder();

            var oldColor = Console.ForegroundColor;

            if (request != null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                var line = $"{request.Method} {request.RequestUri.PathAndQuery} {(request.RequestUri.OriginalString.ToLower().Contains("https") ? Consts.HTTPS : Consts.HTTP)}/{request.ProtocolVersion}";
                output.Append(line);
                Console.WriteLine(line);

                Console.WriteLine();

                if (options.ShowHeaders)
                {
                    for (int idx = 0; idx < request.Headers.Count; idx++)
                    {
                        var key = request.Headers.GetKey(idx);
                        var val = request.Headers[idx];
                       output.Append(WriteColored(key, val));
                    }
                }


                if (options.ShowBody)
                {
                    if (request.ContentType == "application/json")
                    {
                        var obj = JObject.Parse(requestBody);

                        output.Append(WriteJsonTree(obj, 1));
                    }
                    else
                    {
                        Console.ForegroundColor = oldColor;
                        output.Append(requestBody);
                        Console.WriteLine(requestBody);
                    }
                }

            }

            output.Append(Environment.NewLine + Environment.NewLine);
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = oldColor;
            return output.ToString();
        }

        internal static string Write(Options options, RunResult result, HttpWebResponse response)
        {
            var output = new StringBuilder();
            var oldColor = Console.ForegroundColor;
            
            if (result.ResponseCode != 0 && response == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                var line = string.Format("ERROR {0} {1} ",
                              options.Item.Url,
                              result.ResponseCode
                    );
                output.Append(line);
                Console.Write(line);
            }

            if (response != null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                var line = string.Format("{0}/{1} {2} ",
                              (response.ResponseUri.OriginalString.ToLower().Contains("https") ? Consts.HTTPS : Consts.HTTP),
                              response.ProtocolVersion,
                              result.ResponseCode
                    );

                Console.Write(line);
                output.Append(line);

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(response.StatusDescription);
                output.Append(response.StatusDescription + Environment.NewLine);

                if (options.ShowHeaders)
                {
                    foreach (string header in response.Headers)
                    {
                        output.Append(WriteColored(header, response.GetResponseHeader(header)));
                    }
                }

                if (options.ShowBody)
                {
                    if (response.ContentType == "application/json")
                    {
                        var obj = JObject.Parse(result.ResponseBody);

                        output.Append(WriteJsonTree(obj, 1));
                    }
                    else
                    {
                        Console.ForegroundColor = oldColor;
                        output.Append(result.ResponseBody + Environment.NewLine);
                        Console.WriteLine(result.ResponseBody);
                    }
                }
            }

            Console.ForegroundColor = oldColor;

            return output.ToString();
        }

        private static string WriteJsonTree(JObject obj, int intend)
        {
            Console.ForegroundColor = ConsoleColor.White;
            var output = new StringBuilder();


            output.Append("{"+ Environment.NewLine);
            Console.WriteLine("{");

            foreach (var item in obj)
            {
                for (int idx = 1; idx <= intend; idx++)
                {
                    output.Append("  ");
                    Console.Write("  ");
                }
                if (item.Value.Type == JTokenType.Object)
                {
                    var el = $"\"{item.Key}\"";
                    output.Append(el);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(el);

                    output.Append(": ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(": ");

                    output.Append(WriteJsonTree(obj[item.Key] as JObject, intend + 1));

                }
                else
                {
                    string key = $"\"{item.Key}\"";
                    output.Append(key);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(key);

                    output.Append(": ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(": ");

                    string val;
                    if (item.Value.Type == JTokenType.String)
                    {
                        val = $"\"{item.Value}\"";
                        output.Append(val + Environment.NewLine);
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine(val);
                    }
                    else
                    {
                        val = item.Value.ToString();
                        output.Append(val + Environment.NewLine);
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine(val);
                    }
                }
            }


            for (int idx = 1; idx < intend; idx++)
            {
                output.Append("  ");
                Console.Write("  ");
            }

            output.Append("}" + Environment.NewLine);
            Console.ForegroundColor = ConsoleColor.White; 
            Console.WriteLine("}");

            return output.ToString();
        }

       

        private static string WriteColored(string key, string value)
        {
            var output = "";
            Console.ForegroundColor = ConsoleColor.Yellow;
            output += key;
            Console.Write(key);
            Console.ForegroundColor = ConsoleColor.White;
            output += ": ";
            Console.Write(": ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            output += value;
            Console.WriteLine(value);
            return output + Environment.NewLine;
        }
    }
}
