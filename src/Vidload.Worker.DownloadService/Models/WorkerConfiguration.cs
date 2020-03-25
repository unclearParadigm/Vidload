using Vidload.Library.Caching.Models;
using Vidload.Library.Domain.Models.Configuration;
using Vidload.Library.JobQueue.Models;

namespace Vidload.Worker.DownloadService.Models {
  public class WorkerConfiguration {
    public QueueConfiguration JobQueue { get; set; }
    public CacheConfiguration JobStatusCache { get; set; }
    public CacheConfiguration MediaLocationCache { get; set; }
    public NetworkConfiguration NetworkConfiguration { get; set; }
    public FilesystemConfiguration FilesystemConfiguration { get; set; }
  }
}
