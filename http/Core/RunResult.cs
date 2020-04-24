namespace http
{
    public class RunResult
    {
        public string ErrorMessage { get; set; }
        public int ExitCode { get; set; }
        public string OutputMessage { get; set; }
        public string ResponseBody { get; set; }
        public int ResponseCode { get; set; }
    }
}
