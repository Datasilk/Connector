using Microsoft.AspNetCore.Http;

namespace Connector
{
    public class Service : Datasilk.Service
    {
        public Service(HttpContext context) : base(context) { }
    }
}