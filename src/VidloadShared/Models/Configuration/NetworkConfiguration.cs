namespace VidloadShared.Models.Configuration {
  public class NetworkConfiguration {
    public bool UseProxy { get; set; }

    public int ProxyPort { get; set; }
    public string ProxySchema { get; set; }
    public string ProxyHostname { get; set; }
    public string ProxyUsername { get; set; }
    public string ProxyPassword { get; set; }


    public string Proxy =>
      $"{ProxySchema}://{ProxyUsername}:{ProxyPassword}@{ProxyHostname}:{ProxyPort}";
  }
}
