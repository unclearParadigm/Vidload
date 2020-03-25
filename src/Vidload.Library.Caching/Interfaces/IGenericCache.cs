using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Vidload.Library.Caching.Interfaces {
  public interface IGenericCache<T> : IDisposable {
    Task<Result<Maybe<T>>> GetAsync(string key);
    Task<Result> SetAsync(string key, T content);
  }
}
