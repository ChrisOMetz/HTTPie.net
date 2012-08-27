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

    internal static partial class BuildConsts
    {
        public const string ASSEMBLYTITLE = "HTTPie.net";

        public const string COMPILE_MAJOR_VERSION = "0";
        public const string COMPILE_MINOR_VERSION = "2";
        public const string COMPILE_BUILD_NUMBER = "0";
        public const string COMPILE_REVISION = "0";

        /// <summary>
        /// Globale Konstante für die Assembly-Version;
        /// </summary>
        public const string VERSION = COMPILE_MAJOR_VERSION + "." + COMPILE_MINOR_VERSION + "." + COMPILE_BUILD_NUMBER + "." + COMPILE_REVISION;

        public const string MAIN_VERSION = COMPILE_MAJOR_VERSION + "." + COMPILE_MINOR_VERSION;

#if DEBUG
        public const string ASSEMBLYDESCRIPTION = "Debug Version";
#else
			public const string ASSEMBLYDESCRIPTION			= "Release Version";
#endif
#if DEBUG
        public const string ASSEMBLYCONFIGURATION = "Debug";
#else
			public const string ASSEMBLYCONFIGURATION		= "Release";
#endif

        public const string ASSEMBLYCOMPANY = "enjoy;)dialogs";
        public const string ASSEMBLYPRODUCT = "HTTPie.net";
        public const string ASSEMBLYCOPYRIGHT = "Copyright © enjoy;)dialogs 2012";
        public const string ASSEMBLYTRADEMARK = "Trademark © enjoy;)dialogs 2012";
        public const string ASSEMBLYCULTURE = "";

        public const bool ASSEMBLYDELAYSIGN = false;

        public const string FULL_VERSION = "2012.08.27 " + VERSION;
    }
}
