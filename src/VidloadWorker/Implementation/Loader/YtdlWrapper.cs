using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CSharpFunctionalExtensions;
using VidloadShared.Models.Cache;
using VidloadShared.Models.Configuration;
using VidloadShared.Models.Jobs;
using VidloadShared.Structures;
using VidloadWorker.Interfaces;

namespace VidloadWorker.Implementation.Loader {
  public class YouTubeDlVideoInformation {
    [JsonProperty(PropertyName = "fulltitle")]
    public string FullTitle { get; set; }

    [JsonProperty(PropertyName = "creator")]
    public string Creator { get; set; }

    [JsonProperty(PropertyName = "artist")]
    public string Artist { get; set; }

    [JsonProperty(PropertyName = "categories")]
    public List<string> Categories { get; set; }

    [JsonProperty(PropertyName = "description")]
    public string Description { get; set; }

    [JsonProperty(PropertyName = "tags")] public List<string> Tags { get; set; }

    [JsonProperty(PropertyName = "uploader")]
    public string Uploader { get; set; }

    [JsonProperty(PropertyName = "upload_date")]
    public long UploadDate { get; set; }

    [JsonProperty(PropertyName = "thumbnail")]
    public string Thumbnail { get; set; }

    public MediaMetadata ToVideoInformation() {
      return new MediaMetadata {
        Artist = Artist?.Trim(),
        Title = FullTitle?.Trim(),
        Creator = Creator?.Trim(),
        Thumbnail = Thumbnail,
        Description = Description?.Trim(),
        Catgories = Categories?.Select(c => c.Trim()).Where(c => !string.IsNullOrWhiteSpace(c)).ToList(),
        Tags = Tags?.Select(t => t.Trim()).Where(t => !string.IsNullOrWhiteSpace(t)).ToList(),
        UploadDateTime = DateTimeOffset.FromUnixTimeSeconds(UploadDate).DateTime
      };
    }
  }

  public class YtdlWrapper : IVideoLoader {
    private readonly IShellCommandExecutor _shellCommandExecutor;
    private readonly VidloadConfiguration _vidloadConfiguration;

    public YtdlWrapper(IShellCommandExecutor shellCommandExecutor, VidloadConfiguration vidloadConfiguration) {
      _shellCommandExecutor = shellCommandExecutor;
      _vidloadConfiguration = vidloadConfiguration;
    }

    public Task<Result<MediaMetadata>> HandleVideoInformationRequest(MediaMetadataJob request) {
      if (!Uri.TryCreate(request.DownloadLink, UriKind.Absolute, out _))
        return Task.FromResult(Result.Failure<MediaMetadata>("The URL is not valid"));

      var videoInformation = RetrieveJsonVideoInformation(request)
        .Bind(DeserializeFromJson)
        .Bind(ToVideoInfo);

      if (videoInformation.IsSuccess) 
        videoInformation.Value.VideoSource = request.DownloadLink;
      
      return Task.FromResult(videoInformation);
    }

    public Task<Result<MediaLocation>> HandleVideoDownloadRequest(MediaDownloadJob job) {
      if (!Uri.TryCreate(job.DownloadLink, UriKind.Absolute, out _))
        return Task.FromResult(Result.Failure<MediaLocation>("The URL is not valid"));

      var commandlineArguments = new List<string> {
        $"\"{job.DownloadLink}\"",
        "-c",
        "-i",
        "-w",
        "--no-part",
        "--no-call-home",
        $"-o \"{_vidloadConfiguration.FilesystemConfiguration.DownloadDirectory}/{job.TraceId}.%(ext)s\"",
      };

      if (_vidloadConfiguration.NetworkConfiguration.UseProxy) {
        commandlineArguments.Add($"--proxy \"{_vidloadConfiguration.NetworkConfiguration.Proxy}\"");
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
        var filePath = Path.Join(_vidloadConfiguration.FilesystemConfiguration.DownloadDirectory, fileName);

        return Task.FromResult(File.Exists(filePath)
          ? Result.Success(new MediaLocation {FilePath = filePath, TraceId = job.TraceId, DownloadLink = job.DownloadLink})
          : Result.Failure<MediaLocation>("Could not post-process the video. Output File not found"));
      }

      return Task.FromResult(Result.Failure<MediaLocation>("WHOOOP"));
    }

    private Result<string> RetrieveJsonVideoInformation(MediaMetadataJob request) {
      var commandlineArguments = new List<string> {
        $"\"{request.DownloadLink}\"",
        "--skip-download",
        "--print-json",
      };

      if (_vidloadConfiguration.NetworkConfiguration.UseProxy) {
        commandlineArguments.Add($"--proxy \"{_vidloadConfiguration.NetworkConfiguration.Proxy}\"");
      }

      return _shellCommandExecutor
        .Execute("youtube-dl", commandlineArguments, TimeSpan.FromSeconds(5));
    }

    private static Result<YouTubeDlVideoInformation> DeserializeFromJson(string json) {
      try {
        var deserialized = JsonConvert.DeserializeObject<YouTubeDlVideoInformation>(json);
        return Result.Success(deserialized);
      } catch (Exception exc) {
        return Result.Failure<YouTubeDlVideoInformation>(exc.Message);
      }
    }

    private static Result<MediaMetadata> ToVideoInfo(YouTubeDlVideoInformation ytdlVideoInformation) {
      try {
        return Result.Success(ytdlVideoInformation.ToVideoInformation());
      } catch (Exception exc) {
        return Result.Failure<MediaMetadata>("Could not read from Response");
      }
    }
  }
}
