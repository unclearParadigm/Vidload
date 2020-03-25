using System;
using System.Threading.Tasks;

namespace Vidload.Library.JobQueue.Interfaces {
  public interface IDequeuer<T> : IDisposable {
    void Open();
    void OnJobReceived(Func<T, Task> jobReceivedCallback);
  }
}
