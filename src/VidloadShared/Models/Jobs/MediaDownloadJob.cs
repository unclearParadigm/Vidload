using VidloadShared.Structures;

namespace VidloadShared.Models.Jobs {
  public class MediaDownloadJob {
    public string TraceId { get; set; }
    public string UserId { get; set; }
    public string DownloadLink { get; set; }
    public OutputFormat TargetFormat { get; set; }
  }
}
