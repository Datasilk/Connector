using Microsoft.AspNetCore.Http;
using Connector.Models;

namespace Connector.Pages
{
    public class Dashboard: Page
    {
        public Dashboard(HttpContext context) : base(context) { }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            //check security
            if (!CheckSecurity()) { return AccessDenied(true, new Login(context)); }

            //set up client-side dependencies
            AddCSS("/css/views/dashboard/dashboard.css");
            AddScript("js/views/dashboard/dashboard.js");

            //load the dashboard layout
            var scaffold = new Scaffold("/Views/Dashboard/dashboard.html", Server.Scaffold);

            //load UI
            scaffold.Bind(new Header()
            {
                User = new Models.Profile()
                {
                    Name = User.name,
                    Username = User.displayName,
                    Image = User.photo ? "/content/users/" + User.userId + ".jpg" : "/images/nophoto.jpg"
                }
            });

            scaffold.Data["body"] = body;

            //render base layout along with dashboard section
            return base.Render(path, scaffold.Render());
        }
    }
}
