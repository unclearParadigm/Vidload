using System;
using CSharpFunctionalExtensions;
using Newtonsoft.Json;
using Vidload.Library.Serialization.Interfaces;

namespace Vidload.Library.Serialization.Implementations {
  public class JsonSerializer : ISerializer {
    public Result<string> Serialize<T>(T toSerialize) {
      try {
        return Result.Ok(JsonConvert.SerializeObject(toSerialize));
      } catch (Exception exc) {
        return Result.Failure<string>(exc.Message);
      }
    }

    public Result<T> Deserialize<T>(string toDeserialize) {
      try {
        return Result.Ok(JsonConvert.DeserializeObject<T>(toDeserialize));
      } catch (Exception exc) {
        return Result.Failure<T>(exc.Message);
      }
    }
  }
}
