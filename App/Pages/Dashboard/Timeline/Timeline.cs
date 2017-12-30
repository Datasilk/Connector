﻿namespace CoreTemplate.Pages.DashboardPages
{
    public class Timeline: Page
    {
        public Timeline(Core DatasilkCore) : base(DatasilkCore)
        {
        }

        public override string Render(string[] path, string body = "", object metadata = null)
        {
            //load timeline
            var scaffold = new Scaffold(S, "/Pages/Dashboard/Timeline/timeline.html");
            return scaffold.Render();
        }
    }
}
