using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vidload.Library.Caching.Implementations;
using Vidload.Library.Caching.Interfaces;
using Vidload.Library.Domain.Models.Cache;
using Vidload.Library.Domain.Models.Jobs;
using Vidload.Library.JobQueue.Implementations;
using Vidload.Library.JobQueue.Interfaces;
using Vidload.Library.Platform.Implementations;
using Vidload.Library.Platform.Interfaces;
using Vidload.Library.Serialization.Implementations;
using Vidload.Library.Serialization.Interfaces;
using Vidload.Workers.MetadataCollector.Implementations;
using Vidload.Workers.MetadataCollector.Interfaces;
using Vidload.Workers.MetadataCollector.Models;

namespace Vidload.Workers.MetadataCollector {
  internal static class Program {
    private static readonly CancellationTokenSource _cts = new CancellationTokenSource();
    
    public static Task Main() {
      Console.Title = "Vidload.Worker.MetadataCollector";
      Console.CancelKeyPress += (sender, args) => _cts.Cancel(false);

      var workerConfiguration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", true, false)
        .AddEnvironmentVariables()
        .Build()
        .Get<WorkerConfiguration>();
      
      var diContainer = new ServiceCollection()
        .AddSingleton<Worker>()
        .AddSingleton(workerConfiguration)
        .AddSingleton(workerConfiguration.JobQueue)
        .AddSingleton(workerConfiguration.MediaMetadataCache)
        .AddSingleton<ISerializer, JsonSerializer>()
        .AddSingleton<IMediaMetadataCollector, YoutubeDlWrapper>()
        .AddSingleton<IShellCommandExecutor, ShellCommandExecutor>()
        .AddSingleton<IDequeuer<MediaMetadataJob>, RabbitMqDequeuer<MediaMetadataJob>>()
        .AddSingleton<IGenericCache<MediaMetadata>, GenericRedisCache<MediaMetadata>>()
        .BuildServiceProvider();

      var service = diContainer.GetService<Worker>();
      return service.BeginAsync(_cts.Token);
    }
  }
}
