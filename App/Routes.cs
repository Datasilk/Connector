using Datasilk;

public class Routes : Datasilk.Routes
{
    public Routes(Core DatasilkCore) : base(DatasilkCore) { }

    public override Page FromPageRoutes(string name)
    {
        switch (name)
        {
            case "": case "home": return new CoreTemplate.Pages.Home(S);
            case "login": return new CoreTemplate.Pages.Login(S);
        }
        return null;

    }

    public override Service FromServiceRoutes(string name)
    {
        return null;
    }
}
