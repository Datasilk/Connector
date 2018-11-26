using Microsoft.AspNetCore.Http;
using Connector.Models;

namespace Connector.Services
{
    public class BrowseCommunities : Service
    {
        public BrowseCommunities(HttpContext context) : base(context)
        {
        }

        public string View()
        {
            //load partial view
            var scaffold = new Scaffold("/Views/Communities/browse.html");

            //render partial view
            return RenderContent("Browse Communities", "icon-user", scaffold.Render());
        }
    }
}
