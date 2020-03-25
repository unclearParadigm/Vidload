using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using CSharpFunctionalExtensions;
using Vidload.Library.Caching.Interfaces;
using Vidload.Library.Caching.Models;
using Vidload.Library.Serialization.Interfaces;

namespace Vidload.Library.Caching.Implementations {
  public class GenericRedisCache<T> : IGenericCache<T> {
    private readonly ISerializer _serializer;
    private readonly CacheConfiguration _cacheConfiguration;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    
    private const string _notConnectedMessage = "Could not connect to Redis-Instance. Verify that Redis is running an reachable";
    
    public GenericRedisCache(CacheConfiguration cacheConfiguration, ISerializer serializer) {
      VerifyCacheConfigurationSanity(cacheConfiguration);

      _serializer = serializer;
      _cacheConfiguration = cacheConfiguration;
      _connectionMultiplexer = ConnectionMultiplexer.Connect(cacheConfiguration.ConnectionString);
    }

    public GenericRedisCache(CacheConfiguration cacheConfiguration, IConnectionMultiplexer connectionMultiplexer) {
      VerifyCacheConfigurationSanity(cacheConfiguration);
      
      _cacheConfiguration = cacheConfiguration;
      _connectionMultiplexer = connectionMultiplexer;
    }

    private void VerifyCacheConfigurationSanity(CacheConfiguration cacheConfiguration) {
      var criteriaCatalog = new Dictionary<string, bool> {
        {$"{nameof(cacheConfiguration.ConnectionString)} must not be null or empty", string.IsNullOrEmpty(cacheConfiguration.ConnectionString)},
        {$"{nameof(cacheConfiguration.DatabaseKey)} must not be null or empty", string.IsNullOrEmpty(cacheConfiguration.DatabaseKey)},
      };
      
      if(criteriaCatalog.Values.Any(c => c))
        throw new ArgumentException(criteriaCatalog.First(c => c.Value).Key, nameof(cacheConfiguration));
    }

    private bool IsConnected() {
      return _connectionMultiplexer.IsConnected && _connectionMultiplexer.GetDatabase().IsConnected("_");
    }
    
    public async Task<Result<Maybe<T>>> GetAsync(string key) {
      if (!IsConnected())
        return Result.Failure<Maybe<T>>(_notConnectedMessage);
      
      var db = _connectionMultiplexer.GetDatabase();
      var data = await db.HashGetAsync(_cacheConfiguration.DatabaseKey, key);
      if (!data.HasValue) return Result.Success(Maybe<T>.None);
      return _serializer
        .Deserialize<T>(data)
        .Map(Maybe<T>.From);
    }

    public async Task<Result> SetAsync(string key, T content) {
      if (!IsConnected())
        return Result.Failure(_notConnectedMessage);

      try {
        var db = _connectionMultiplexer.GetDatabase();
        var maybeSerialized = _serializer.Serialize(content);
        if (maybeSerialized.IsFailure) return Result.Failure(maybeSerialized.Error);
        await db.HashSetAsync(_cacheConfiguration.DatabaseKey, key, maybeSerialized.Value);
        return Result.Success();
      } catch (Exception exc) {
        return Result.Failure(exc.Message);
      }
    }

    public void Dispose() {
      _connectionMultiplexer?.Dispose();
    }
  }
}
