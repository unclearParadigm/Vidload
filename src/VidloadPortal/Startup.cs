using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using VidloadCache;
using VidloadShared.Models.Configuration;

namespace VidloadPortal {
  public class Startup {
    private readonly VidloadConfiguration _vidloadConfiguration;

    public Startup(IConfiguration configuration) {
      _vidloadConfiguration = configuration.Get<VidloadConfiguration>();
    }

    public void ConfigureServices(IServiceCollection services) {
      var cm = ConnectionMultiplexer.Connect(_vidloadConfiguration.CacheConfiguration.RedisConnectionString);
      services.AddSingleton(_vidloadConfiguration);
      services.AddSingleton(_vidloadConfiguration.CacheConfiguration);
      services.AddSingleton<IConnectionMultiplexer>(cm);
      services.AddSingleton<IVidloadCache, RedisVidloadCache>();
      services
        .AddMvc()
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
      app.UseStaticFiles();
      app.UseCookiePolicy();

      app.UseMvc(routes => {
        routes.MapRoute(
          name: "default",
          template: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}
