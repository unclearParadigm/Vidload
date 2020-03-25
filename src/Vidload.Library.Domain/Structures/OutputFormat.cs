using System;
using System.Linq;

namespace Vidload.Library.Domain.Structures {
  public enum OutputFormat {
    Mp3,
    Wav,
    Flac,
    Vorbis,
    Mp4,
    Webm
  }

  public static class FormatSpecifier {
    public static readonly OutputFormat[] VideoFormats = new[]
      {OutputFormat.Mp4, OutputFormat.Webm};

    public static readonly OutputFormat[] AudioFormats = new[]
      {OutputFormat.Mp3, OutputFormat.Vorbis, OutputFormat.Wav, OutputFormat.Flac};

    public static bool IsAudioFormat(OutputFormat outputFormat) {
      return AudioFormats.Contains(outputFormat);
    }

    public static bool IsVideoFormat(OutputFormat outputFormat) {
      return VideoFormats.Contains(outputFormat);
    }

    public static string GetFileExtensionsForFormat(OutputFormat outputFormat) {
      switch (outputFormat) {
        case OutputFormat.Mp3:
        case OutputFormat.Wav:
        case OutputFormat.Flac:
        case OutputFormat.Mp4:
        case OutputFormat.Webm:
          return outputFormat.ToString().ToLower();

        case OutputFormat.Vorbis:
          return "ogg";

        default:
          throw new ArgumentOutOfRangeException(nameof(outputFormat), outputFormat, null);
      }
    }
  }
}
