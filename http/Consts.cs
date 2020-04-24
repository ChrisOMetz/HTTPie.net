using System;

namespace http
{
    public static class Consts
    {
        public const string HTTP = "HTTP";
        public const string HTTPS = "HTTPS";
        public const string HTTP_POST = "POST";
        public const string HTTP_GET = "GET";
        public const string HTTP_DELETE = "DELETE";
        public const string HTTP_PUT = "PUT";

        public static class EXIT
        {
            public const int OK = 0;
            public const int ERROR = 1;
            public const int ERROR_TIMEOUT = 2;

            public const int ERROR_HTTP_3XX = 3;
            public const int ERROR_HTTP_4XX = 4;
            public const int ERROR_HTTP_5XX = 5;
        }
    }
}
