using Vidload.Library.Caching.Models;
using Vidload.Library.Domain.Models.Configuration;
using Vidload.Library.JobQueue.Models;

namespace Vidload.Workers.MetadataCollector.Models {
  public class WorkerConfiguration {
    public QueueConfiguration JobQueue { get; set; }
    public CacheConfiguration MediaMetadataCache { get; set; }
    public NetworkConfiguration NetworkConfiguration { get; set; }
  }
}
