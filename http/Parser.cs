using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace http
{
    public class Options
    {
        public FormatOption Format { get; set; }
        public RequestItem Item { get; set; }
        public bool CheckStatus { get; set; }
        public bool AllowRedirects { get; set; }
        public bool UseForm { get; set; }
        public bool UseJson { get; set; }
        
    }

    public class FormatOption
    {

    }

    public class RequestItem
    {
        public string Url { get; set; }
        public string Method { get; set; }
        public IList<string> Headers { get; set; }
        //public string[] Data { get; set; }
        //public bool Verify { get; set; }
        public int Timeout { get; set; }
        //public string[] Auth { get; set; }
        //public string[] Files { get; set; }
        public IList<string> Paramters { get; set; }
        public IList<string> QueryStringParameters { get; set; }
    }

    internal static class Parser
    {

        public static Options ParseArgs(string[] args)
        {
            var arguments = ConvertArguments(args);
            var format = ProcessPrettyOptions(args);
            var item = ParseArguments(args);

            return new Options
                {
                    Format = format,
                    Item = item,
                    CheckStatus = true,
                    AllowRedirects = false,
                    UseForm = arguments.Contains("--form"),
                    UseJson = arguments.Contains("--json")
                };
        }

        private static List<string> ConvertArguments(string[] args)
        {
            var result = new List<string>();

            foreach (String option in args)
            {
                if (option.StartsWith("--"))
                {
                    if (option.Equals("--form", StringComparison.CurrentCultureIgnoreCase) ||
                        option.Equals("--f", StringComparison.CurrentCultureIgnoreCase))
                    {
                        result.Add("--form");
                    }
                    else if (option.Equals("--json", StringComparison.CurrentCultureIgnoreCase) ||
                             option.Equals("--j", StringComparison.CurrentCultureIgnoreCase))
                    {
                        result.Add("--json");
                    }
                }
            }

            return result;
        }

        private static FormatOption ProcessPrettyOptions(string[] args)
        {
            var result = new FormatOption();
            return result;
        }

        private static RequestItem ParseArguments(string[] options)
        {
            var result = new RequestItem();
            bool methodSet = false;
            bool urlSet = false;

            foreach (String option in options)
            {
                if (!option.StartsWith("-"))
                {
                    bool found = false;

                    if (!methodSet)
                    {
                        if (option.Equals(Consts.HTTP_GET, StringComparison.OrdinalIgnoreCase))
                        {
                            result.Method = Consts.HTTP_GET;
                            methodSet = true;
                            found = true;
                        }
                        else if (option.Equals(Consts.HTTP_POST, StringComparison.OrdinalIgnoreCase))
                        {
                            result.Method = Consts.HTTP_POST;
                            methodSet = true;
                            found = true;
                        }
                        else if (option.Equals(Consts.HTTP_PUT, StringComparison.OrdinalIgnoreCase))
                        {
                            result.Method = Consts.HTTP_PUT;
                            methodSet = true;
                            found = true;
                        }
                        else if (option.Equals(Consts.HTTP_DELETE, StringComparison.OrdinalIgnoreCase))
                        {
                            result.Method = Consts.HTTP_DELETE;
                            methodSet = true;
                            found = true;
                        }
                    }


                    if (!found)
                    {
                        if (!urlSet)
                        {
                            string url;
                            //This must be the url
                            if (!option.ToLower().StartsWith("http://") && !option.ToLower().StartsWith("https://") && !option.ToLower().StartsWith("ws://"))
                                url = "http://" + option;
                            else
                                url = option;

                            if (!IsWebUrl(url))
                            {
                                throw new ArgumentException("Not a valid url!");
                            }
                            result.Url = url;
                            urlSet = true;
                        }
                        else
                        {

                            if (option.Contains("=="))
                            {
                                // Querystring Parameters
                                if (result.QueryStringParameters == null)
                                    result.QueryStringParameters = new List<string> { option.Replace("==", "=") };
                                else
                                    result.QueryStringParameters.Add(option.Replace("==", "="));
                            }
                            else if (option.Contains("="))
                            {
                                //Parameters
                                if (result.Paramters == null)
                                    result.Paramters = new List<string> {option};
                                else
                                    result.Paramters.Add(option);
                            }
                            else if (option.Contains(":"))
                            {
                                //Header
                                if (result.Headers == null)
                                    result.Headers = new List<string> { option };
                                else
                                    result.Headers.Add(option);
                            }
                        }
                        
                    }

                }
            }

            if (!methodSet)
            {
                result.Method = result.Paramters == null ? Consts.HTTP_GET : Consts.HTTP_POST;
            }

            return result;
        }


        private static readonly Regex webUrlExpression = new Regex(@"(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", RegexOptions.Singleline | RegexOptions.Compiled);
        private static bool IsWebUrl(string target)
        {
            return !string.IsNullOrEmpty(target) && webUrlExpression.IsMatch(target);
        }
    }
}
