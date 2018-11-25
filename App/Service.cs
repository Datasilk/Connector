using Microsoft.AspNetCore.Http;

namespace Connector
{
    public class Service : Datasilk.Service
    {
        public Service(HttpContext context) : base(context) { }

        public string RenderContent(string title, string html, string js = "", string jsFile = "", string cssFile = "")
        {
            var response = new Models.Service.Response()
            {
                title = title,
                html = html,
                js = js,
                jsfile = jsFile,
                cssfile = cssFile
            };
            return Utility.Serialization.Serializer.WriteObjectToString(response);
        }
    }
}