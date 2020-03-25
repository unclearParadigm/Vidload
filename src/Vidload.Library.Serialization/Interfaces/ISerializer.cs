using CSharpFunctionalExtensions;

namespace Vidload.Library.Serialization.Interfaces {
  public interface ISerializer {
    Result<string> Serialize<T>(T toSerialize);
    Result<T> Deserialize<T>(string toDeserialize);
  }
}
