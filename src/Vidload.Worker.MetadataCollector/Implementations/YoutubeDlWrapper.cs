using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Newtonsoft.Json;
using Vidload.Library.Domain.Models.Cache;
using Vidload.Library.Domain.Models.Jobs;
using Vidload.Library.Platform.Interfaces;
using Vidload.Library.Serialization.Interfaces;
using Vidload.Workers.MetadataCollector.Interfaces;
using Vidload.Workers.MetadataCollector.Models;

namespace Vidload.Workers.MetadataCollector.Implementations {
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

  public class YoutubeDlWrapper : IMediaMetadataCollector {
    private readonly IShellCommandExecutor _shellCommandExecutor;
    private readonly WorkerConfiguration _workerConfiguration;
    private readonly ISerializer _serializer;

    public YoutubeDlWrapper(ISerializer serializer, IShellCommandExecutor shellCommandExecutor, WorkerConfiguration workerConfiguration) {
      _shellCommandExecutor = shellCommandExecutor;
      _workerConfiguration = workerConfiguration;
      _serializer = serializer;
    }

    public Task<Result<MediaMetadata>> HandleMediaMetadataCollection(MediaMetadataJob request) {
      if (!Uri.TryCreate(request.DownloadLink, UriKind.Absolute, out _))
        return Task.FromResult(Result.Failure<MediaMetadata>("The URL is not valid"));

      var videoInformation = RetrieveJsonVideoInformation(request)
        .Bind(json => _serializer.Deserialize<YouTubeDlVideoInformation>(json))
        .Bind(ToVideoInfo);

      if (videoInformation.IsSuccess) 
        videoInformation.Value.VideoSource = request.DownloadLink;
      
      return Task.FromResult(videoInformation);
    }
    private Result<string> RetrieveJsonVideoInformation(MediaMetadataJob request) {
      var commandlineArguments = new List<string> {
        $"\"{request.DownloadLink}\"",
        "--skip-download",
        "--print-json",
      };

      if (_workerConfiguration.NetworkConfiguration.UseProxy) {
        commandlineArguments.Add($"--proxy \"{_workerConfiguration.NetworkConfiguration.Proxy}\"");
      }

      return _shellCommandExecutor
        .Execute("youtube-dl", commandlineArguments, TimeSpan.FromSeconds(5));
    }
    
    private static Result<MediaMetadata> ToVideoInfo(YouTubeDlVideoInformation ytdlVideoInformation) {
      try {
        return Result.Success(ytdlVideoInformation.ToVideoInformation());
      } catch (Exception exc) {
        return Result.Failure<MediaMetadata>($"Could not read from Response: {exc.Message}");
      }
    }
  }
}
