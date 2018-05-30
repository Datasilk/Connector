using Microsoft.AspNetCore.Http;
using Datasilk;

public class Routes : Datasilk.Routes
{

    public override Page FromPageRoutes(HttpContext context, string name)
    {
        switch (name)
        {
            case "": case "home": return new CoreTemplate.Pages.Home(context);
            case "login": return new CoreTemplate.Pages.Login(context);
        }
        return null;

    }

    public override Service FromServiceRoutes(HttpContext context, string name)
    {
        return null;
    }
}
