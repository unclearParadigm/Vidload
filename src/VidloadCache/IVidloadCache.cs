using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using VidloadShared.Models.Cache;
using VidloadShared.Structures;

namespace VidloadCache {
  public interface IVidloadCache : IDisposable {
    Task<Result> SetMetadata(string downloadLink, MediaMetadata mediaMetadata);
    Task<Result<Maybe<MediaMetadata>>> GetMetadata(string downloadLink);

    Task<Result> SetJobStatus(string downloadLink, JobStatus jobStatus);
    Task<Result<Maybe<JobStatus>>> GetJobStatus(string downloadLink);

    Task<Result> SetMediaLocation(string downloadLink, MediaLocation mediaLocation);
    Task<Result<Maybe<MediaLocation>>> GetMediaLocation(string downloadLink);
  }
}
