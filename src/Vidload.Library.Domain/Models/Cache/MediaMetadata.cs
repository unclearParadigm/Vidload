using System;
using System.Collections.Generic;

namespace Vidload.Library.Domain.Models.Cache {
  public class MediaMetadata {
    public string Title { get; set; }
    public string Artist { get; set; }
    public string Creator { get; set; }
    public string Thumbnail { get; set; }
    public DateTime UploadDateTime { get; set; }
    public List<string> Catgories { get; set; }
    public string Description { get; set; }
    public List<string> Tags { get; set; }

    public string VideoSource { get; set; }
  }
}
