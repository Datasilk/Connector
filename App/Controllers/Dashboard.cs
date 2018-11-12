using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Connector.Pages
{
    public class Dashboard: Page
    {
        public struct structMenuItem
        {
            public string label;
            public string id;
            public string href;
            public string icon;
            public List<structMenuItem> submenu;
        }

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
            var scaffMenu = new Scaffold("/Views/Dashboard/menu-item.html", Server.Scaffold);

            //load user profile
            scaffold.Data["profile-img"] = "";
            scaffold.Data["btn-edit-img"] = "";
            scaffold.Data["profile-name"] = User.displayName;

            //load website info
            scaffold.Data["website-name"] = title;
            scaffold.Data["website-url"] = "http://connector.datasilk.io";
            scaffold.Data["website-url-name"] = "connector.datasilk.io";

            //generate menu system
            var menu = new StringBuilder();
            var menus = new List<structMenuItem>()
            {
                menuItem("Timeline", "timeline", "/dashboard/timeline", "timeline")
            };

            //render menu system
            foreach (var item in menus)
            {
                menu.Append(renderMenuItem(scaffMenu, item, 0));
            }
            scaffold.Data["menu"] = "<ul class=\"menu\">" + menu.ToString() + "</ul>";

            //get dashboard section name
            var subPath = context.Request.Path.ToString().Replace("dashboard", "").Substring(1);
            if(subPath == "" || subPath == "/") { subPath = "timeline"; }
            var html = "";

            //load dashboard section
            Page subpage = null;
            var t = LoadSubPage(subPath);
            subpage = t.Item1;
            html = t.Item2;
            if (html == "") { return AccessDenied(true, new Login(context)); }
            scaffold.Data["body"] = html;

            //set up page info
            title = title + " - Dashboard - " + subpage.title;

            //include dashboard section javascript dependencies
            scripts.Append(subpage.scripts);

            //render base layout along with dashboard section
            return base.Render(path, scaffold.Render());
        }

        private Tuple<Page, string> LoadSubPage(string path)
        {
            //get correct sub page from path
            Page service = null;
            var html = "";
            var paths = path.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            var subpath = paths.Skip(1).ToArray();

            if (paths[0] == "timeline")
            {
                service = new DashboardPages.Timeline(context);
            }
            
            //render sub page
            html = service.Render(subpath);

            return new Tuple<Page, string>(service, html);
        }

        private structMenuItem menuItem(string label, string id, string href, string icon, List<structMenuItem> submenu = null)
        {
            var menu = new structMenuItem();
            menu.label = label;
            menu.id = id;
            menu.href = href;
            menu.icon = icon;
            menu.submenu = submenu;
            return menu;
        }

        private string renderMenuItem(Scaffold scaff, structMenuItem item, int level = 0)
        {
            var gutter = "";
            var subs = new StringBuilder();
            for (var x = 0; x < level; x++)
            {
                gutter += "<div class=\"gutter\"></div>";
            }
            if (item.submenu != null)
            {
                if(item.submenu.Count > 0)
                {
                    foreach(var sub in item.submenu)
                    {
                        subs.Append(renderMenuItem(scaff, sub, level + 1));
                    }
                }
            }
            scaff.Data["label"] = item.label;
            scaff.Data["href"] = item.href == "" ? "javascript:" : item.href;
            scaff.Data["section-name"] = item.id;
            scaff.Data["icon"] = item.icon;
            scaff.Data["gutter"] = gutter;
            if(subs.Length > 0)
            {
                scaff.Data["target"] = " target=\"_self\"";
                scaff.Data["submenu"] = "<div class=\"row submenu\"><ul class=\"menu\">" + subs.ToString() + "</ul></div>";
            }
            else
            {
                scaff.Data["submenu"] = "";
            }
            
            return scaff.Render();
        }
    }
}
