using Microsoft.AspNetCore.Http;

namespace Connector.Pages
{
    public class Subscribers : Dashboard
    {
        public Subscribers(HttpContext context) : base(context)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            return base.Render(path, body, metadata);
        }
    }
}
