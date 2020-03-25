using System;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using CSharpFunctionalExtensions;

using Vidload.Library.JobQueue.Models;
using Vidload.Library.JobQueue.Interfaces;
using Vidload.Library.Serialization.Interfaces;

namespace Vidload.Library.JobQueue.Implementations {
  public class RabbitMqDequeuer<T> : IDequeuer<T> {
    private IModel _rabbitMqChannel;
    private IConnection _rabbitMqConnection;
    private Func<T, Task> onJobReceivedCallback;
    private EventingBasicConsumer eventingBasicConsumer;

    private readonly ISerializer _serializer;
    private readonly QueueConfiguration _queueConfiguration;
    
    public RabbitMqDequeuer(QueueConfiguration queueConfiguration, ISerializer serializer) {
      _queueConfiguration = queueConfiguration;
      _serializer = serializer;
    }

    public void OnJobReceived(Func<T, Task> jobReceivedCallback) {
      this.onJobReceivedCallback = jobReceivedCallback;
    }

    public void Open() {
      if (onJobReceivedCallback == null)
        throw new MissingMethodException("Cannot open. Please provide a callback.");

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

      eventingBasicConsumer = new EventingBasicConsumer(_rabbitMqChannel);

      _rabbitMqChannel.BasicConsume(
        queue: _queueConfiguration.QueueName,
        autoAck: true,
        consumer: eventingBasicConsumer);
      
      eventingBasicConsumer.Received += (model, ea) => {
        DeserializeJobQueueMessage(ea.Body)
          .Tap(m => onJobReceivedCallback(m));
      };
    }

    public void Close() {
      _rabbitMqChannel?.Close();
      _rabbitMqConnection?.Close();
    }

    private  Result<T> DeserializeJobQueueMessage(byte[] body) {
      if(body == null || body.Length == 0)
        return Result.Failure<T>("Empty message received");

      var message = Encoding.UTF8.GetString(body);
      return _serializer.Deserialize<T>(message);
    }

    public void Dispose() {
      _rabbitMqChannel?.Dispose();
      _rabbitMqConnection?.Dispose();
    }
  }
}
