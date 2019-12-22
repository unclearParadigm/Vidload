using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;
using CSharpFunctionalExtensions;
using VidloadShared.Structures;
using VidloadShared.Models.Cache;
using VidloadShared.Models.Configuration;

namespace VidloadCache {
  public class RedisVidloadCache : IVidloadCache {
    private readonly CacheConfiguration _cacheConfiguration;
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisVidloadCache(CacheConfiguration cacheConfiguration) {
      _cacheConfiguration = cacheConfiguration;
      _connectionMultiplexer = ConnectionMultiplexer.Connect(cacheConfiguration.RedisConnectionString);
    }

    public RedisVidloadCache(CacheConfiguration cacheConfiguration, IConnectionMultiplexer connectionMultiplexer) {
      _cacheConfiguration = cacheConfiguration;
      _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<Result> SetMetadata(string downloadLink, MediaMetadata mediaMetadata) {
      if (!IsConnected())
        return Result.Failure("Could not connect to VidloadCache");

      try {
        var db = _connectionMultiplexer.GetDatabase();
        var serialized = JsonConvert.SerializeObject(mediaMetadata);
        await db.HashSetAsync(_cacheConfiguration.MetadataKey, downloadLink, serialized);
        return Result.Success();
      } catch (Exception exc) {
        return Result.Failure(exc.Message);
      }
    }

    public async Task<Result<Maybe<MediaMetadata>>> GetMetadata(string downloadLink) {
      if (!IsConnected())
        return Result.Failure<Maybe<MediaMetadata>>("Could not connect to VidloadCache");

      try {
        var db = _connectionMultiplexer.GetDatabase();
        var data = await db.HashGetAsync(_cacheConfiguration.MetadataKey, downloadLink);
        if (!data.HasValue) return Result.Success(Maybe<MediaMetadata>.None);
        var deserialized = JsonConvert.DeserializeObject<MediaMetadata>(data);
        return Result.Success(Maybe<MediaMetadata>.From(deserialized));
      } catch (Exception exc) {
        return Result.Failure<Maybe<MediaMetadata>>(exc.Message);
      }
    }

    public async Task<Result> SetJobStatus(string downloadLink, JobStatus jobStatus) {
      if (!IsConnected())
        return Result.Failure("Could not connect to VidloadCache");

      try {
        var db = _connectionMultiplexer.GetDatabase();
        await db.HashSetAsync(_cacheConfiguration.JobStatusKey, downloadLink, jobStatus.ToString());
        return Result.Success();
      } catch (Exception exc) {
        return Result.Failure(exc.Message);
      }
    }

    public async Task<Result<Maybe<JobStatus>>> GetJobStatus(string downloadLink) {
      if (!IsConnected())
        return Result.Failure<Maybe<JobStatus>>("Could not connect to VidloadCache");

      try {
        var db = _connectionMultiplexer.GetDatabase();
        var data = await db.HashGetAsync(_cacheConfiguration.JobStatusKey, downloadLink);
        if (!data.HasValue) return Result.Success(Maybe<JobStatus>.None);
        var jobStatus = (JobStatus)Enum.Parse(typeof(JobStatus), data, true);
        return Result.Success(Maybe<JobStatus>.From(jobStatus));
      } catch (Exception exc) {
        return Result.Failure<Maybe<JobStatus>>(exc.Message);
      }
    }

    public async Task<Result> SetMediaLocation(string downloadLink, MediaLocation mediaLocation) {
      if (!IsConnected())
        return Result.Failure("Could not connect to VidloadCache");

      try {
        var db = _connectionMultiplexer.GetDatabase();
        var serialized = JsonConvert.SerializeObject(mediaLocation);
        await db.HashSetAsync(_cacheConfiguration.MediaLocationKey, downloadLink, serialized);
        return Result.Success();
      } catch (Exception exc) {
        return Result.Failure(exc.Message);
      }
    }

    public async Task<Result<Maybe<MediaLocation>>> GetMediaLocation(string downloadLink) {
      if (!IsConnected())
        return Result.Failure<Maybe<MediaLocation>>("Could not connect to VidloadCache");

      try {
        var db = _connectionMultiplexer.GetDatabase();
        var data = await db.HashGetAsync(_cacheConfiguration.MediaLocationKey, downloadLink);
        if (!data.HasValue) return Result.Success(Maybe<MediaLocation>.None);
        var deserialized = JsonConvert.DeserializeObject<MediaLocation>(data);
        return Result.Success(Maybe<MediaLocation>.From(deserialized));
      } catch (Exception exc) {
        return Result.Failure<Maybe<MediaLocation>>(exc.Message);
      }
    }

    private bool IsConnected() {
      return _connectionMultiplexer.IsConnected && _connectionMultiplexer.GetDatabase().IsConnected("_");
    }

    public void Dispose() {
      _connectionMultiplexer?.Dispose();
    }
  }
}
