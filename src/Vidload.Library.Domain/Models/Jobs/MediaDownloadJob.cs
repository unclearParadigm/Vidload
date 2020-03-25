using Vidload.Library.Domain.Structures;

namespace Vidload.Library.Domain.Models.Jobs {
  public class MediaDownloadJob {
    public string TraceId { get; set; }
    public string UserId { get; set; }
    public string DownloadLink { get; set; }
    public OutputFormat TargetFormat { get; set; }
  }
}
