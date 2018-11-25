namespace Connector.Models.Service
{
    public enum ResponseType
    {
        generic = 0,
        content = 1
    }
    public class Response
    {
        public ResponseType type { get; set; }
        public string title { get; set; }
        public string icon { get; set; }
        public string html { get; set; }
        public string js { get; set; }
        public string jsfile { get; set; }
        public string cssfile { get; set; }
    }
}
