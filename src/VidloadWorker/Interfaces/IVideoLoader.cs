using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using VidloadShared.Models.Cache;
using VidloadShared.Models.Jobs;

namespace VidloadWorker.Interfaces {
  public interface IVideoLoader {
    Task<Result<MediaMetadata>> HandleVideoInformationRequest(MediaMetadataJob request);
    Task<Result<MediaLocation>> HandleVideoDownloadRequest(MediaDownloadJob job);
  }
}
