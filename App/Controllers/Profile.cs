using Microsoft.AspNetCore.Http;

namespace Connector.Pages
{
    public class Profile : Dashboard
    {
        public Profile(HttpContext context) : base(context)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            return base.Render(path, body, metadata) +
                "<script>S.dashboard.tab.open('Profile');</script>";
        }
    }
}
