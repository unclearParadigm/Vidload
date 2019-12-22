namespace VidloadShared.Models.Configuration {
  public class CacheConfiguration {
    public string RedisConnectionString { get; set; }
    public string MediaLocationKey { get; set; }
    public string JobStatusKey { get; set; }
    public string MetadataKey { get; set; }
  }
}
