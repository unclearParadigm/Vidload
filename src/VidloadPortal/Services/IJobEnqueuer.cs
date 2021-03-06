using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using VidloadShared.Models.Jobs;

namespace VidloadPortal.Services {
  public interface IJobEnqueuer : IDisposable {
    void Open();
    Task<Result> Enqueue(MediaMetadataJob mediaMetadataJob);
    Task<Result> Enqueue(MediaDownloadJob mediaDownloadJob);
  }
}
