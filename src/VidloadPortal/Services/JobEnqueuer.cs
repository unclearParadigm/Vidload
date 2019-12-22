using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using VidloadShared.Models.Jobs;

namespace VidloadPortal.Services {
  public class JobEnqueuer : IJobEnqueuer {
    public Task<Result> Enqueue(MediaMetadataJob mediaMetadataJob) {
      throw new System.NotImplementedException();
    }

    public Task<Result> Enqueue(MediaDownloadJob mediaDownloadJob) {
      throw new System.NotImplementedException();
    }

    public void Dispose() {
    }
  }
}
