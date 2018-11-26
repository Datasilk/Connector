using Microsoft.AspNetCore.Http;

namespace Connector.Pages
{
    public class Subscriptions : Dashboard
    {
        public Subscriptions(HttpContext context) : base(context)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            return base.Render(path, body, metadata);
        }
    }
}
