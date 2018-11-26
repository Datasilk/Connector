using Microsoft.AspNetCore.Http;

namespace Connector.Pages
{
    public class BrowseCommunities : Dashboard
    {
        public BrowseCommunities(HttpContext context) : base(context)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            return base.Render(path, body, metadata);
        }
    }
}
