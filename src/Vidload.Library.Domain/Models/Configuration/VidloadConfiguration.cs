namespace Vidload.Library.Domain.Models.Configuration {
  public class VidloadConfiguration {
    public JobQueueConfiguration JobQueueConfiguration { get; set; }
    public FilesystemConfiguration FilesystemConfiguration { get; set; }
    public NetworkConfiguration NetworkConfiguration { get; set; }
  }
}
