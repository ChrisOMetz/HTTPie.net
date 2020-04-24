namespace http
{
    public class FileParameter
    {
        public byte[] File { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public FileParameter(byte[] file, string filename, string contenttype = null)
        {
            File = file;
            FileName = filename;
            ContentType = contenttype;
        }
    }
}
