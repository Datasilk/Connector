using Microsoft.AspNetCore.Http;
using Connector.Models;

namespace Connector.Services
{
    public class Subscribers : Service
    {
        public Subscribers(HttpContext context) : base(context)
        {
        }

        public string View()
        {
            //load partial view
            var scaffold = new Scaffold("/Views/Subscribers/subscribers.html");

            //render partial view
            return RenderContent("Subscribers", "icon-user", scaffold.Render());
        }
    }
}
