using Microsoft.AspNetCore.Http;
using Connector.Models;

namespace Connector.Services
{
    public class Subscriptions : Service
    {
        public Subscriptions(HttpContext context) : base(context)
        {
        }

        public string View()
        {
            //load partial view
            var scaffold = new Scaffold("/Views/Subscriptions/subscriptions.html");

            //render partial view
            return RenderContent("My Subscriptions", "icon-user", scaffold.Render());
        }
    }
}
