using System;
using System.Net;
using Newtonsoft.Json.Linq;

namespace http
{
    internal static class Output
    {
        internal static void Write(Options options, RunResult result, HttpWebResponse response)
        {
            var oldColor = Console.ForegroundColor;

            if (result.ResponseCode != 0 && response == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("ERROR {0} {1} ",
                              options.Item.Url,
                              result.ResponseCode
                    );

            }

            if (response != null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("{0} {1}/{2} {3} ",
                              response.Method,
                              (response.ResponseUri.OriginalString.ToLower().Contains("https") ? Consts.HTTPS : Consts.HTTP),
                              response.ProtocolVersion,
                              result.ResponseCode
                    );

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(response.StatusDescription);

                foreach (string header in response.Headers)
                {
                    WriteColored(header, response.GetResponseHeader(header));
                }

                if (response.ContentType == "application/json")
                {
                    var obj = JObject.Parse(result.ResponseBody);

                    WriteJsonTree(obj, 1);
                }
                else
                {
                    Console.ForegroundColor = oldColor;
                    Console.WriteLine(result.ResponseBody);
                }
            }

            Console.ForegroundColor = oldColor;
        }

        private static void WriteJsonTree(JObject obj, int intend)
        {
            Console.ForegroundColor = ConsoleColor.White;
    
            Console.WriteLine("{");

            foreach (var item in obj)
            {
                for (int idx = 1; idx <= intend; idx++)
                    Console.Write("  ");
                if (item.Value.Type == JTokenType.Object)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("\"{0}\"", item.Key);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(": ");
                    WriteJsonTree(obj[item.Key] as JObject, intend + 1);
                    
                }
                else
                {
                    string key = string.Format("\"{0}\"", item.Key);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(key);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(": ");

                    string val = "";
                    if (item.Value.Type == JTokenType.String)
                    {
                        val = string.Format("\"{0}\"", item.Value);
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine(val);
                    }
                    else
                    {
                        val = item.Value.ToString();
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine(val);
                    }
                }
            }


            for (int idx = 1; idx < intend; idx++)
                Console.Write("  ");

            Console.ForegroundColor = ConsoleColor.White; 
            Console.WriteLine("}");
            
        }

       

        private static void WriteColored(string key, string value)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(key);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(": ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(value);
        }
    }
}
