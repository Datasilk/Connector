using Microsoft.AspNetCore.Http;
using Connector.Models;

namespace Connector.Services
{
    public class Profile : Service
    {
        public Profile(HttpContext context) : base(context)
        {
        }

        public string View()
        {
            //load partial view
            var scaffold = new Scaffold("/Views/Profile/profile.html");

            //bind model
            scaffold.Bind(new Header()
            {
                User = new Models.Profile()
                {
                    Name = User.name,
                    Username = User.displayName,
                    Image = User.photo ? "/content/users/" + User.userId + ".jpg" : "/images/nophoto.jpg"
                }
            });

            //render partial view
            return RenderContent(User.name, scaffold.Render());
        }
    }
}
