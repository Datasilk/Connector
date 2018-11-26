using System.Collections.Generic;
using System.Text;
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
            var menuitem = new Scaffold("/Views/Dashboard/menu-item.html", Server.Scaffold);

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


            //load Options Menu UI
            var menuItems = new List<OptionsMenuItem>()
            {
                new OptionsMenuItem()
                {
                    Id = "find-friends",
                    Href = "Find-Friends",
                    Icon = "icon-user",
                    Label = "Find Friends"
                },
                new OptionsMenuItem()
                {
                    Id = "browse-communities",
                    Href = "Browse-Communities",
                    Icon = "icon-communities",
                    Label = "Browse Communities"
                },
                new OptionsMenuItem()
                {
                    Id = "subscriptions",
                    Href = "Subscriptions",
                    Icon = "icon-subscriptions",
                    Label = "My Subscriptions"
                },
                new OptionsMenuItem()
                {
                    Id = "subscribers",
                    Href = "Subscribers",
                    Icon = "icon-subscribers",
                    Label = "Subscribers"
                }
            };

            var optionMenu = new StringBuilder();
            foreach(var item in menuItems)
            {
                menuitem.Bind(item);
                optionMenu.Append(menuitem.Render());
            }
            scaffold.Data["options-menu"] = optionMenu.ToString();

            scaffold.Data["body"] = body;

            //render base layout along with dashboard section
            return base.Render(path, scaffold.Render());
        }
    }
}
