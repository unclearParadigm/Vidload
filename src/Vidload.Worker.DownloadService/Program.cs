using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Vidload.Library.Caching.Implementations;
using Vidload.Library.Caching.Interfaces;
using Vidload.Library.Domain.Models.Cache;
using Vidload.Library.Domain.Models.Jobs;
using Vidload.Library.Domain.Structures;
using Vidload.Library.JobQueue.Implementations;
using Vidload.Library.JobQueue.Interfaces;
using Vidload.Library.Platform.Implementations;
using Vidload.Library.Platform.Interfaces;
using Vidload.Library.Serialization.Implementations;
using Vidload.Library.Serialization.Interfaces;
using Vidload.Worker.DownloadService.Implementations;
using Vidload.Worker.DownloadService.Interfaces;
using Vidload.Worker.DownloadService.Models;

namespace Vidload.Worker.DownloadService {
  internal static class Program {
    private static readonly CancellationTokenSource _cts = new CancellationTokenSource();

    private static Task Main() {
      Console.Title = "Vidload.Worker.DownloadService";
      Console.CancelKeyPress += (sender, args) => _cts.Cancel(false);

      var workerConfiguration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", true, false)
        .AddEnvironmentVariables()
        .Build()
        .Get<WorkerConfiguration>();

      var serializationMethod = new JsonSerializer();
      
      var diContainer = new ServiceCollection()
        .AddSingleton<Worker>()
        .AddSingleton(workerConfiguration)
        .AddSingleton(workerConfiguration.JobQueue)
        .AddSingleton<ISerializer>(serializationMethod)
        .AddSingleton<IMediaDownloader, YoutubeDlWrapper>()
        .AddSingleton<IShellCommandExecutor, ShellCommandExecutor>()
        .AddSingleton<IDequeuer<MediaDownloadJob>, RabbitMqDequeuer<MediaDownloadJob>>()
        .AddSingleton<IGenericCache<JobStatus>>(new GenericRedisCache<JobStatus>(workerConfiguration.JobStatusCache, serializationMethod))
        .AddSingleton<IGenericCache<MediaLocation>>(new GenericRedisCache<MediaLocation>(workerConfiguration.MediaLocationCache, serializationMethod))
        .BuildServiceProvider();

      var service = diContainer.GetService<Worker>();
      return service.BeginAsync(_cts.Token);
    }
  }
}
