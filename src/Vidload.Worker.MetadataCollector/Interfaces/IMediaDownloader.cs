using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Vidload.Library.Domain.Models.Cache;
using Vidload.Library.Domain.Models.Jobs;

namespace Vidload.Workers.MetadataCollector.Interfaces {
  public interface IMediaMetadataCollector {
    Task<Result<MediaMetadata>> HandleMediaMetadataCollection(MediaMetadataJob request);
  }
}
