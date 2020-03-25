using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Vidload.Library.Domain.Models.Cache;
using Vidload.Library.Domain.Models.Jobs;
using Vidload.Library.Domain.Structures;
using Vidload.Library.Platform.Interfaces;
using Vidload.Worker.DownloadService.Interfaces;
using Vidload.Worker.DownloadService.Models;

namespace Vidload.Worker.DownloadService.Implementations {
  public class YoutubeDlWrapper : IMediaDownloader {
    private readonly IShellCommandExecutor _shellCommandExecutor;
    private readonly WorkerConfiguration _workerConfiguration;

    public YoutubeDlWrapper(IShellCommandExecutor shellCommandExecutor, WorkerConfiguration workerConfiguration) {
      _shellCommandExecutor = shellCommandExecutor;
      _workerConfiguration = workerConfiguration;
    }

    public Task<Result<MediaLocation>> HandleMediaDownload(MediaDownloadJob job) {
      if (!Uri.TryCreate(job.DownloadLink, UriKind.Absolute, out _))
        return Task.FromResult(Result.Failure<MediaLocation>("The URL is not valid"));

      var commandlineArguments = new List<string> {
        $"\"{job.DownloadLink}\"",
        "-c",
        "-i",
        "-w",
        "--no-part",
        "--no-call-home",
        $"-o \"{_workerConfiguration.FilesystemConfiguration.DownloadDirectory}/{job.TraceId}.%(ext)s\"",
      };

      if (_workerConfiguration.NetworkConfiguration.UseProxy) {
        commandlineArguments.Add($"--proxy \"{_workerConfiguration.NetworkConfiguration.Proxy}\"");
      }


      if (FormatSpecifier.IsAudioFormat(job.TargetFormat)) {
        commandlineArguments.Add("--extract-audio");
        commandlineArguments.Add($"--audio-format {job.TargetFormat.ToString().ToLower()}");
        commandlineArguments.Add("-f \"bestaudio\"");
      }

      if (FormatSpecifier.IsVideoFormat(job.TargetFormat)) {
        commandlineArguments.Add("-f \"bestvideo\"");
        commandlineArguments.Add($"--recode-video {job.TargetFormat.ToString().ToLower()}");
      }

      var downloadResult = _shellCommandExecutor
        .Execute("youtube-dl", commandlineArguments, TimeSpan.FromHours(1));

      if (downloadResult.IsSuccess) {
        var fileName = $"{job.TraceId}.{FormatSpecifier.GetFileExtensionsForFormat(job.TargetFormat)}";
        var filePath = Path.Join(_workerConfiguration.FilesystemConfiguration.DownloadDirectory, fileName);

        return Task.FromResult(File.Exists(filePath)
          ? Result.Success(new MediaLocation {FilePath = filePath, TraceId = job.TraceId, DownloadLink = job.DownloadLink})
          : Result.Failure<MediaLocation>("Could not post-process the video. Output File not found"));
      }

      Console.WriteLine(downloadResult.Error);    
      return Task.FromResult(Result.Failure<MediaLocation>(downloadResult.Error));
    }
  }
}
