namespace Connector.Models.UI
{
    public class Header
    {
        public HeaderUser User { get; set; }
    }

    public class HeaderUser
    {
        public string Image { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
    }
}
