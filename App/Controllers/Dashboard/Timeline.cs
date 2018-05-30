using Microsoft.AspNetCore.Http;

namespace CoreTemplate.Pages.DashboardPages
{
    public class Timeline: Page
    {
        public Timeline(HttpContext context) : base(context)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            //load timeline
            var scaffold = new Scaffold("/Views/Dashboard/Timeline/timeline.html", Server.Scaffold);
            return scaffold.Render();
        }
    }
}
