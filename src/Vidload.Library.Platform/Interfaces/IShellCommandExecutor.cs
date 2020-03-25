using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace Vidload.Library.Platform.Interfaces {
  public interface IShellCommandExecutor {
    Result<string> Execute(string executable, IEnumerable<string> cmdParameters, TimeSpan timeout);
  }
}
