namespace Connector.Models
{
    public class Header
    {
        public Profile User { get; set; }
    }

    public class OptionsMenuItem
    {
        public string Id { get; set; }
        public string Href { get; set; }
        public string Icon { get; set; }
        public string Label { get; set; }
    }
}
