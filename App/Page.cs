using Microsoft.AspNetCore.Http;

namespace Connector
{
    public class Page : Datasilk.Page
    {
        public Page(HttpContext context) : base(context)
        {
            title = "Connector";
            description = "Connect with people around the world";
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            return base.Render(path, body, metadata);
        }

        public void LoadHeader(ref Scaffold scaffold)
        {
            if(User.userId > 0)
            {
                scaffold.Child("header").Data["user"] = "1";
            }
            else
            {
                scaffold.Child("header").Data["no-user"] = "1";
            }
        }
    }
}