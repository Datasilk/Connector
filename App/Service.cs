using Microsoft.AspNetCore.Http;

namespace CoreTemplate
{
    public class Service : Datasilk.Service
    {
        public Service(HttpContext context) : base(context) { }
    }
}