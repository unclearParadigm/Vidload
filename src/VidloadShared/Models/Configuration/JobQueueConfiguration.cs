namespace VidloadShared.Models.Configuration {
  public class JobQueueConfiguration {
    public string VideoDownloadJobQueueName { get; set; }
    public string VideoInformationJobQueueName { get; set; }

    public string JobQueueHostname { get; set; }
    public string JobQueueUsername { get; set; }
    public string JobQueuePassword { get; set; }

    public int JobQueuePort { get; set; }
  }
}
