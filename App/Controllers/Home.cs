using Microsoft.AspNetCore.Http;

namespace CoreTemplate.Pages
{
    public class Home : Page
    {
        public Home(HttpContext context) : base(context)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            var scaffold = new Scaffold("/Views/Home/home.html", Server.Scaffold);
            
            if(User.userId > 0)
            {
                scaffold.Data["user"] = "1";
                scaffold.Data["username"] = User.name;
            }
            else
            {
                scaffold.Data["no-user"] = "1";
            }

            //load header since it was included in home.html
            LoadHeader(ref scaffold);

            //add CSS file for home
            AddCSS("/css/views/home/home.css");

            //finally, render base page layout with home page
            return base.Render(path, scaffold.Render(), metadata);
        }
    }
}
