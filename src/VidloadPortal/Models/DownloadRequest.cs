using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using VidloadShared.Structures;

namespace VidloadPortal.Models {
  public class DownloadRequest {
    [JsonProperty(PropertyName = "downloadLink")]
    public string DownloadLink { get; set; }
    [JsonProperty(PropertyName = "outputFormat")]
    public string OutputFormat { get; set; }

    public bool IsValid() {
      var validUrlChars = new[] {'/', ':', '.', '=', '?', '-', '_', '+', '#', '&', '[', ']'};

      var validityCriteria = new[] {
        DownloadLink != null,
        OutputFormat != null,
        !string.IsNullOrWhiteSpace(DownloadLink),
        !string.IsNullOrWhiteSpace(OutputFormat),
        DownloadLink?.Length >= 8,
        OutputFormat?.Length >= 3,
        DownloadLink?.All(c => char.IsDigit(c) || char.IsLetter(c) || validUrlChars.Contains(c)),
        Uri.TryCreate(DownloadLink, UriKind.Absolute, out _),
        Enum.TryParse(typeof(OutputFormat), OutputFormat, true, out _),
      };

      return validityCriteria.All(c => c == true);
    }
  }
}
