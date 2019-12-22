using System;
using System.Threading.Tasks;
using VidloadShared.Models.Jobs;

namespace VidloadWorker.Interfaces {
  public interface IJobQueueReceiver : IDisposable {
    void Open();
    void Close();
    void RegisterCallback(Func<MediaDownloadJob, Task> handler);
    void RegisterCallback(Func<MediaMetadataJob, Task> handler);
  }
}
