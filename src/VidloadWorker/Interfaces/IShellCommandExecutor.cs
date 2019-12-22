using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace VidloadWorker.Interfaces {
  public interface IShellCommandExecutor {
    Result<string> Execute(string executable, IEnumerable<string> cmdParameters, TimeSpan timeout);
  }
}
