using Microsoft.AspNetCore.Http;
using Connector.Models;

namespace Connector.Services
{
    public class FindFriends : Service
    {
        public FindFriends(HttpContext context) : base(context)
        {
        }

        public string View()
        {
            //load partial view
            var scaffold = new Scaffold("/Views/FindFriends/findfriends.html");

            //render partial view
            return RenderContent("Find Friends", "icon-user", scaffold.Render());
        }
    }
}
