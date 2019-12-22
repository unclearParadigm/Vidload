namespace VidloadShared.Models.Configuration {
  public class VidloadConfiguration {
    public JobQueueConfiguration JobQueueConfiguration { get; set; }
    public FilesystemConfiguration FilesystemConfiguration { get; set; }
    public NetworkConfiguration NetworkConfiguration { get; set; }
    public CacheConfiguration CacheConfiguration { get; set; }
  }
}
