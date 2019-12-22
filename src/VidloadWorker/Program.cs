using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VidloadCache;
using VidloadShared.Models.Configuration;
using VidloadWorker.Interfaces;
using VidloadWorker.Implementation;
using VidloadWorker.Implementation.Loader;

namespace VidloadWorker {
  internal static class Program {
    private static CancellationTokenSource _cts = new CancellationTokenSource();

    private static async Task Main() {
      Console.Title = "VidloadWorker";
      Console.CancelKeyPress += (sender, args) => _cts.Cancel(false);

      var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", true, false)
        .AddEnvironmentVariables()
        .Build()
        .Get<VidloadConfiguration>();

      var diContainer = new ServiceCollection()
        .AddSingleton(configuration)
        .AddSingleton<IVideoLoader, YtdlWrapper>()
        .AddSingleton<IVidloadCache, RedisVidloadCache>()
        .AddSingleton<IJobQueueReceiver, RabbitMqJobQueueReceiver>()
        .AddSingleton<IShellCommandExecutor, ShellCommandExecutor>()
        .AddSingleton<VidloadWorkerService>()
        .BuildServiceProvider();

      var service = diContainer.GetService<VidloadWorkerService>();
      await service.BeginAsync(_cts.Token);
    }
  }
}
