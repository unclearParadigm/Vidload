using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Vidload.Library.Caching.Interfaces;
using Vidload.Library.Domain.Models.Cache;
using Vidload.Library.Domain.Models.Jobs;
using Vidload.Library.JobQueue.Interfaces;
using Vidload.Workers.MetadataCollector.Interfaces;

namespace Vidload.Workers.MetadataCollector {
  public class Worker {
    private readonly IMediaMetadataCollector _mediaMetadataCollector;
    private readonly IDequeuer<MediaMetadataJob> _dequeuer;
    private readonly IGenericCache<MediaMetadata> _mediaMetadataCache;
    
    public Worker(
      IMediaMetadataCollector mediaMetadataCollector,
      IDequeuer<MediaMetadataJob> dequeuer,
      IGenericCache<MediaMetadata> mediaMetadataCache) {
      _mediaMetadataCollector = mediaMetadataCollector;
      _dequeuer = dequeuer;
      _mediaMetadataCache = mediaMetadataCache;
    }

    public async Task BeginAsync(CancellationToken cancellationToken) {
      using (_dequeuer) {
        _dequeuer.OnJobReceived(HandleMetadataDownloadRequest);
        _dequeuer.Open();

        while (!cancellationToken.IsCancellationRequested) {
          await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
        }
      }
    }
    
    private async Task HandleMetadataDownloadRequest(MediaMetadataJob downloadJob) {
      await _mediaMetadataCollector.HandleMediaMetadataCollection(downloadJob)
        .Bind(ml => _mediaMetadataCache.SetAsync(downloadJob.TraceId, ml));
    }
  }
}
