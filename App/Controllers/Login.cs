using Microsoft.AspNetCore.Http;

namespace Connector.Pages
{
    public class Login: Page
    {
        public Login(HttpContext context) : base(context) { }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            if(User.userId > 0)
            {
                //redirect to dashboard
                return base.Render(path, Redirect("/dashboard/"));
            }

            //check for database reset
            var scaffold = new Scaffold("/Views/Login/login.html", Server.Scaffold);

            if(Server.hasAdmin == false)
            {
                //load new administrator form
                scaffold = new Scaffold("/Views/Login/new-user.html", Server.Scaffold);
                scaffold.Data["title"] = "Create a user account";
                scripts.Append("<script src=\"/js/views/login/new-user.js\"></script>");
            }
            else
            {
                //load login form (default)
                scripts.Append("<script src=\"/js/views/login/login.js\"></script>");
            }

            //load login page
            return base.Render(path, scaffold.Render());
        }
    }
}
