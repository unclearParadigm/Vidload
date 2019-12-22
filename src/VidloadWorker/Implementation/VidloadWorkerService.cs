using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using VidloadCache;
using VidloadShared.Models.Jobs;
using VidloadShared.Structures;
using VidloadWorker.Interfaces;

namespace VidloadWorker.Implementation {
  public class VidloadWorkerService {
    private readonly IVideoLoader _videoLoader;
    private readonly IVidloadCache _vidloadCache;
    private readonly IJobQueueReceiver _jobQueueReceiver;


    public VidloadWorkerService(
      IVideoLoader videoLoader,
      IVidloadCache vidloadCache,
      IJobQueueReceiver jobQueueReceiver) {
      _videoLoader = videoLoader;
      _vidloadCache = vidloadCache;
      _jobQueueReceiver = jobQueueReceiver;
    }

    private async Task HandleDownloadRequest(MediaDownloadJob downloadJob) {
      await _vidloadCache.SetJobStatus(downloadJob.TraceId, JobStatus.InProgress)
        .Bind(() => _videoLoader.HandleVideoDownloadRequest(downloadJob))
        .Bind(ml =>
          _vidloadCache.SetMediaLocation(downloadJob.DownloadLink, ml)
            .Bind(() => _vidloadCache.SetJobStatus(downloadJob.TraceId, JobStatus.Completed)))
        .OnFailure(r => _vidloadCache.SetJobStatus(downloadJob.TraceId, JobStatus.Error));
    }

    private async Task HandleVideoInformationRequest(MediaMetadataJob metadataRequest) {
      await _videoLoader.HandleVideoInformationRequest(metadataRequest)
        .Bind(vi => _vidloadCache.SetMetadata(metadataRequest.DownloadLink, vi));
    }

    public async Task BeginAsync(CancellationToken cancellationToken) {
      _jobQueueReceiver.RegisterCallback(HandleDownloadRequest);
      _jobQueueReceiver.RegisterCallback(HandleVideoInformationRequest);
      _jobQueueReceiver.Open();

      while (!cancellationToken.IsCancellationRequested) {
        await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
      }

      _jobQueueReceiver.Close();
    }
  }
}
