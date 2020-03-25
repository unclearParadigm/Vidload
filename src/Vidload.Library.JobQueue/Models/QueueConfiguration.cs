namespace Vidload.Library.JobQueue.Models {
  public class QueueConfiguration {
    public string QueueClientName { get; set; }
    public string QueueHostname { get; set; }
    public string QueueUsername { get; set; }
    public string QueuePassword { get; set; }
    public string QueueName { get; set; }
    public int QueuePort { get; set; }
  }
}
