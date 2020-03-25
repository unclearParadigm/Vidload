using System;
using Vidload.Library.Serialization.Interfaces;

namespace Vidload.Library.Caching.Models {
  public class CacheConfiguration {
    public string ConnectionString { get; set; }
    public string DatabaseKey { get; set; }
  }
}
