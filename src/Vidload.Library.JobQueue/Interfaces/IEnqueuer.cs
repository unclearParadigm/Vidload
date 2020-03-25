using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Vidload.Library.JobQueue.Interfaces {
  public interface IEnqueuer<T> : IDisposable {
    void Open();
    Task<Result> EnqueueAsync(T queueMessage);
  }
}
