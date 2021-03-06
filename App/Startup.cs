using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

public class Startup : Datasilk.Startup {

    public override void Configured(IApplicationBuilder app, IHostingEnvironment env, IConfigurationRoot config)
    {
        base.Configured(app, env, config);
        app.UseHsts();
        app.UseHttpsRedirection();
        Query.Sql.connectionString = server.sqlConnectionString;
        server.hasAdmin = Query.Users.Exist();
    }
}
