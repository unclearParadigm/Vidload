using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Vidload.Library.Domain.Models.Cache;
using Vidload.Library.Domain.Models.Jobs;

namespace Vidload.Worker.DownloadService.Interfaces {
  public interface IMediaDownloader {
    Task<Result<MediaLocation>> HandleMediaDownload(MediaDownloadJob job);
  }
}
