using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace http.Tests
{
    public abstract class SimpleTestsBase
    {
        private const string HTTPBIN_URL = "http://httpbin.org";

        public const string OK = "HTTP/1.1 200";

        public readonly string TEST_ROOT;
        public readonly string FILE_PATH;
        public readonly string FILE2_PATH;
        public readonly string BIN_FILE_PATH;

        public readonly string FILE_PATH_ARG;
        public readonly string FILE2_PATH_ARG;
        public readonly string BIN_FILE_PATH_ARG;

        public readonly string FILE_CONTENT;
        public readonly byte[] BIN_FILE_CONTENT;

        public SimpleTestsBase()
        {
            TEST_ROOT =  Directory.GetCurrentDirectory();
            TEST_ROOT = TEST_ROOT.Substring(0, TEST_ROOT.IndexOf("http.Tests") + 10);

            FILE_PATH = Path.Combine(TEST_ROOT, "files", "file.txt");
            FILE2_PATH = Path.Combine(TEST_ROOT, "files", "file2.txt");
            BIN_FILE_PATH = Path.Combine(TEST_ROOT, "files", "file.bin");

            FILE_PATH_ARG = PathArg(FILE_PATH);
            FILE2_PATH_ARG = PathArg(FILE2_PATH);
            BIN_FILE_PATH_ARG = PathArg(BIN_FILE_PATH);

            FILE_CONTENT = File.ReadAllText(FILE_PATH).Trim();
            BIN_FILE_CONTENT = File.ReadAllBytes(BIN_FILE_PATH);

        }


        protected static string httpbin(string path)
        {
            return HTTPBIN_URL + path;
        }

        protected static RunResult http(string[] args)
        {
            //Invoke with 'args' and 'kwargs'
            var result = new RunResult();

            try
            {
                result = Core.Run(args);
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                result.ExitCode = Consts.EXIT.ERROR;
            }

            return result;
        }

        private static string PathArg(string path)
        {
            // Back slashes need to be escaped in ITEM args, even in Windows paths.
            return path.Replace("\\", "\\\\\\");
        }
    }
}
