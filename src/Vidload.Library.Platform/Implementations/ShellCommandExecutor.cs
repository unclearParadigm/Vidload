using System;
using System.Diagnostics;
using System.Collections.Generic;

using CSharpFunctionalExtensions;
using Vidload.Library.Platform.Interfaces;

namespace Vidload.Library.Platform.Implementations {
  public class ShellCommandExecutor : IShellCommandExecutor {
    public Result<string> Execute(string executable, IEnumerable<string> cmdParameters, TimeSpan timeout) {
      var parameters = string.Join(' ', cmdParameters);

      var processInfo = new ProcessStartInfo {
        CreateNoWindow = true,
        ErrorDialog = false,
        FileName = executable,
        UseShellExecute = false,
        Arguments = parameters,
        RedirectStandardOutput = true
      };

      try {
        using (var process = new Process()) {
          process.StartInfo = processInfo;
          process.Start();
          process.WaitForExit(Convert.ToInt32(timeout.TotalMilliseconds));
          var output = process.StandardOutput.ReadToEnd();

          return process.ExitCode != 0
            ? Result.Failure<string>($"{executable} reported non-zero status code: '{process.ExitCode}'")
            : Result.Success(output);
        }
      } catch (Exception exc) {
        return Result.Failure<string>(exc.Message);
      }
    }
  }
}
