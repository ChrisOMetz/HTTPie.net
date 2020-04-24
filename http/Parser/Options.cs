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
        public bool IsVerbose { get; set; }
        public bool ShowHelp { get; set; }
        public bool ShowHeaders { get; set; }
        public bool ShowBody { get; set; }
        
    }
}
