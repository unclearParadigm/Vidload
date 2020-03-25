using System;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client;
using CSharpFunctionalExtensions;

using Vidload.Library.JobQueue.Models;
using Vidload.Library.JobQueue.Interfaces;
using Vidload.Library.Serialization.Interfaces;

namespace Vidload.Library.JobQueue.Implementations {
  public class RabbitMqEnqueuer<T> : IEnqueuer<T> {
    private IModel _rabbitMqChannel;
    private IConnection _rabbitMqConnection;
    private readonly QueueConfiguration _queueConfiguration;
    private readonly ISerializer _serializer;

    public RabbitMqEnqueuer(QueueConfiguration queueConfiguration, ISerializer serializer) {
      _queueConfiguration = queueConfiguration;
      _serializer = serializer;
    }

    public void Open() {
      var mqConnectionFactory = new ConnectionFactory {
        HostName = _queueConfiguration.QueueHostname,
        UserName = _queueConfiguration.QueueUsername,
        Password = _queueConfiguration.QueuePassword,
        Port = _queueConfiguration.QueuePort
      };

      _rabbitMqConnection = mqConnectionFactory.CreateConnection(_queueConfiguration.QueueClientName);
      _rabbitMqChannel = _rabbitMqConnection.CreateModel();

      _rabbitMqChannel.QueueDeclare(
        queue: _queueConfiguration.QueueName,
        durable: false,
        exclusive: false,
        autoDelete: false,
        arguments: null);
    }

    public Task<Result> EnqueueAsync(T queueMessage) {

      var (_, isFailure, value, error) = _serializer.Serialize(queueMessage)
        .Map(s => Encoding.UTF8.GetBytes(s));

      if (isFailure) return Task.FromResult(Result.Failure(error));

      try {
        _rabbitMqChannel.BasicPublish(
          string.Empty,
          _queueConfiguration.QueueName,
          null,
          value
        );
        return Task.FromResult(Result.Success());
      } catch (Exception exc) {
        return Task.FromResult(Result.Failure(exc.Message));
      }
    }

    public void Dispose() {
      _rabbitMqChannel?.Dispose();
      _rabbitMqConnection?.Dispose();
    }
  }
}
