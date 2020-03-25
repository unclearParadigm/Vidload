using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Vidload.Library.Caching.Interfaces;
using Vidload.Library.Domain.Models.Cache;
using Vidload.Library.Domain.Models.Jobs;
using Vidload.Library.Domain.Structures;
using Vidload.Library.JobQueue.Interfaces;
using Vidload.Worker.DownloadService.Interfaces;

namespace Vidload.Worker.DownloadService {
  public class Worker {
    private readonly IMediaDownloader _mediaDownloader;
    private readonly IDequeuer<MediaDownloadJob> _dequeuer;
    
    private readonly IGenericCache<JobStatus> _jobStatusCache;
    private readonly IGenericCache<MediaLocation> _mediaLocationCache;
    
    public Worker(
      IMediaDownloader mediaDownloader,
      IDequeuer<MediaDownloadJob> dequeuer,
      IGenericCache<JobStatus> jobStatusCache,
      IGenericCache<MediaLocation> mediaLocationCache) {
      _mediaDownloader = mediaDownloader;
      _dequeuer = dequeuer;

      _jobStatusCache = jobStatusCache;
      _mediaLocationCache = mediaLocationCache;
    }

    public async Task BeginAsync(CancellationToken cancellationToken) {
      using (_dequeuer) {
        _dequeuer.OnJobReceived(HandleDownloadRequest);
        _dequeuer.Open();

        while (!cancellationToken.IsCancellationRequested) {
          await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
        }
      }
    }
    
    private async Task HandleDownloadRequest(MediaDownloadJob downloadJob) {
      await _jobStatusCache.SetAsync(downloadJob.TraceId, JobStatus.InProgress)
        .Bind(() => _mediaDownloader.HandleMediaDownload(downloadJob))
        .Bind(ml =>
          _mediaLocationCache.SetAsync(downloadJob.DownloadLink, ml)
            .Bind(() => _jobStatusCache.SetAsync(downloadJob.TraceId, JobStatus.Completed)))
        .OnFailure(r => _jobStatusCache.SetAsync(downloadJob.TraceId, JobStatus.Error));
    }
  }
}
