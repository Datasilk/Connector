using Microsoft.AspNetCore.Http;
using Connector.Models;

namespace Connector.Services
{
    public class Timeline : Service
    {
        public Timeline(HttpContext context) : base(context)
        {
        }

        public string View()
        {
            //load partial view
            var scaffold = new Scaffold("/Views/Timeline/timeline.html");

            //render partial view
            return RenderContent("Timeline", "icon-timeline", scaffold.Render());
        }
    }
}
