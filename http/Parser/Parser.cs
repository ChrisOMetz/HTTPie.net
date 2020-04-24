using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace http
{

    internal static class Parser
    {

        public static Options ParseArgs(string[] args)
        {
            var arguments = ConvertArguments(args);
            var format = ProcessPrettyOptions(args);
            var item = ParseArguments(args);

            //if (item.Files != null && !arguments.Contains("--form"))
            //{
            //    throw new ArgumentException("Invalid file fields (perhaps you meant --form?)");
            //}


            return new Options
                {
                    Format = format,
                    Item = item,
                    CheckStatus = true,
                    AllowRedirects = false,
                    UseForm = arguments.Contains("--form"),
                    UseJson = arguments.Contains("--json"),
                    IsVerbose = arguments.Contains("--verbose"),
                    ShowHeaders = !arguments.Contains("--body"),
                    ShowBody = !arguments.Contains("--headers"),
                    ShowHelp= arguments.Contains("--help")
                };
        }

        private static List<string> ConvertArguments(string[] args)
        {
            var result = new List<string>();

            foreach (var option in args)
            {
                if (option.StartsWith("--", StringComparison.InvariantCulture))
                {
                    if (option.Equals("--form", StringComparison.CurrentCultureIgnoreCase) ||
                        option.Equals("--f", StringComparison.CurrentCultureIgnoreCase) ||
                        option.Equals("/form", StringComparison.CurrentCultureIgnoreCase) ||
                        option.Equals("/f", StringComparison.CurrentCultureIgnoreCase))
                    {
                        result.Add("--form");
                    }
                    else if (option.Equals("--json", StringComparison.CurrentCultureIgnoreCase) ||
                             option.Equals("--j", StringComparison.CurrentCultureIgnoreCase) ||
                             option.Equals("/json", StringComparison.CurrentCultureIgnoreCase) ||
                             option.Equals("/j", StringComparison.CurrentCultureIgnoreCase))
                    {
                        result.Add("--json");
                    }
                    else if (option.Equals("--verbose", StringComparison.CurrentCultureIgnoreCase) ||
                             option.Equals("/verbose", StringComparison.CurrentCultureIgnoreCase))
                    {
                        result.Add("--verbose");
                    }
                    else if (option.Equals("--headers", StringComparison.CurrentCultureIgnoreCase) ||
                         option.Equals("--h", StringComparison.CurrentCultureIgnoreCase) ||
                         option.Equals("/headers", StringComparison.CurrentCultureIgnoreCase) ||
                         option.Equals("/h", StringComparison.CurrentCultureIgnoreCase))
                    {
                        result.Add("--headers");
                    }
                    else if (option.Equals("--body", StringComparison.CurrentCultureIgnoreCase) ||
                     option.Equals("--b", StringComparison.CurrentCultureIgnoreCase) ||
                     option.Equals("/body", StringComparison.CurrentCultureIgnoreCase) ||
                     option.Equals("/b", StringComparison.CurrentCultureIgnoreCase))
                    {
                        result.Add("--body");
                    }
                    else if (option.Equals("--help", StringComparison.CurrentCultureIgnoreCase) ||
                         option.Equals("--h", StringComparison.CurrentCultureIgnoreCase) ||
                         option.Equals("/help", StringComparison.CurrentCultureIgnoreCase) ||
                         option.Equals("/h", StringComparison.CurrentCultureIgnoreCase) ||
                         option.Equals("/?", StringComparison.CurrentCultureIgnoreCase))
                    {
                        result.Add("--help");
                    }
                    else if (option.Equals("--help", StringComparison.CurrentCultureIgnoreCase) ||
                             option.Equals("--h", StringComparison.CurrentCultureIgnoreCase) ||
                             option.Equals("/help", StringComparison.CurrentCultureIgnoreCase) ||
                             option.Equals("/h", StringComparison.CurrentCultureIgnoreCase) ||
                             option.Equals("/?", StringComparison.CurrentCultureIgnoreCase))
                    {
                        result.Add("--help");
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

            foreach (var option in options)
            {
                if (!option.StartsWith("-", StringComparison.InvariantCulture))
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

                            if (option.Contains("==", StringComparison.InvariantCulture))
                            {
                                // Querystring Parameters
                                if (result.QueryStringParameters == null)
                                    result.QueryStringParameters = new List<string> { option.Replace("==", "=") };
                                else
                                    result.QueryStringParameters.Add(option.Replace("==", "="));
                            }
                            else if (option.Contains("=", StringComparison.InvariantCulture))
                            {
                                //Parameters
                                if (result.Paramters == null)
                                    result.Paramters = new List<string> {option};
                                else
                                    result.Paramters.Add(option);
                            }
                            else if (option.Contains("@", StringComparison.InvariantCulture))
                            {
                                //Files
                                //check if file exists
                                var fileInfo = option.Split('@');

                                if (fileInfo.Length == 2)
                                {
                                    if (!File.Exists(fileInfo[1]))
                                    {
                                        throw new FileNotFoundException($"file '{fileInfo[1]}' does not exist.");
                                    }


                                    if (result.Files == null)
                                        result.Files = new List<string> { option };
                                    else
                                        result.Files.Add(option);

                                }
                            }
                            else if (option.Contains(":", StringComparison.InvariantCulture))
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
