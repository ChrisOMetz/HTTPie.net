using System.Collections.Generic;

namespace http
{
    public class RequestItem
    {
        public string Url { get; set; }
        public string Method { get; set; }
        public IList<string> Headers { get; set; }
        //public string[] Data { get; set; }
        //public bool Verify { get; set; }
        public int Timeout { get; set; }
        //public string[] Auth { get; set; }
        public IList<string> Files { get; set; }
        public IList<string> Paramters { get; set; }
        public IList<string> QueryStringParameters { get; set; }
    }
}
