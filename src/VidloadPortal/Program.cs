using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace VidloadPortal {
  public static class Program {
    public static void Main(string[] args) {
      CreateWebHostBuilder(args)
        .Build()
        .Run();
    }

    private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
      WebHost
        .CreateDefaultBuilder(args)
        .UseKestrel(kestrelOptions => {
          kestrelOptions.ListenAnyIP(5000);
        })
        .UseStartup<Startup>();
  }
}
