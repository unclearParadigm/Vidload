using System;
using System.Text;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using VidloadShared.Models.Configuration;
using VidloadShared.Models.Jobs;
using VidloadWorker.Interfaces;

namespace VidloadWorker.Implementation {
  public class RabbitMqJobQueueReceiver : IJobQueueReceiver {
    private IModel _rabbitMqChannel;
    private IConnection _rabbitMqConnection;
    private readonly VidloadConfiguration _vidloadConfiguration;

    private EventingBasicConsumer VideoDownloadJobConsumer;
    private EventingBasicConsumer VideoInformationJobConsumer;

    private Func<MediaDownloadJob, Task> mediaDownloadHandler;
    private Func<MediaMetadataJob, Task> mediaMetadataHandler;

    public RabbitMqJobQueueReceiver(VidloadConfiguration vidloadConfiguration) {
      _vidloadConfiguration = vidloadConfiguration;
    }

    public void RegisterCallback(Func<MediaDownloadJob, Task> handler) {
      mediaDownloadHandler = handler;
    }

    public void RegisterCallback(Func<MediaMetadataJob, Task> handler) {
      mediaMetadataHandler = handler;
    }

    public void Open() {
      if (mediaDownloadHandler == null)
        throw new MissingMethodException("Cannot open. Please provide a callback.");
      if (mediaMetadataHandler == null)
        throw new MissingMethodException("Cannot open. Please provide a callback.");

      var mqConnectionFactory = new ConnectionFactory {
        HostName = _vidloadConfiguration.JobQueueConfiguration.JobQueueHostname,
        UserName = _vidloadConfiguration.JobQueueConfiguration.JobQueueUsername,
        Password = _vidloadConfiguration.JobQueueConfiguration.JobQueuePassword,
        Port = _vidloadConfiguration.JobQueueConfiguration.JobQueuePort
      };

      _rabbitMqConnection = mqConnectionFactory.CreateConnection();
      _rabbitMqChannel = _rabbitMqConnection.CreateModel();

      _rabbitMqChannel.QueueDeclare(
        queue: _vidloadConfiguration.JobQueueConfiguration.MediaDownloadJobQueueName,
        durable: false,
        exclusive: false,
        autoDelete: false,
        arguments: null);

      _rabbitMqChannel.QueueDeclare(
        queue: _vidloadConfiguration.JobQueueConfiguration.MediaMetadataJobQueueName,
        durable: false,
        exclusive: false,
        autoDelete: false,
        arguments: null);

      VideoDownloadJobConsumer = new EventingBasicConsumer(_rabbitMqChannel);
      VideoInformationJobConsumer = new EventingBasicConsumer(_rabbitMqChannel);

      _rabbitMqChannel.BasicConsume(
        queue: _vidloadConfiguration.JobQueueConfiguration.MediaDownloadJobQueueName,
        autoAck: true,
        consumer: VideoDownloadJobConsumer);

      _rabbitMqChannel.BasicConsume(
        queue: _vidloadConfiguration.JobQueueConfiguration.MediaMetadataJobQueueName,
        autoAck: true,
        consumer: VideoInformationJobConsumer);

      VideoDownloadJobConsumer.Received += (model, ea) => {
        DeserializeJobQueueMessage<MediaDownloadJob>(ea.Body)
          .OnSuccessTry(m => mediaDownloadHandler(m));
      };

      VideoInformationJobConsumer.Received += (model, ea) => {
        DeserializeJobQueueMessage<MediaMetadataJob>(ea.Body)
          .Tap(m => mediaMetadataHandler(m));
      };
    }

    public void Close() {
      _rabbitMqChannel?.Close();
      _rabbitMqConnection?.Close();
    }

    private static Result<T> DeserializeJobQueueMessage<T>(byte[] body) {
      try {
        var message = Encoding.UTF8.GetString(body);
        var deserialized = JsonConvert.DeserializeObject<T>(message);
        return Result.Success(deserialized);
      } catch {
        return Result.Failure<T>("Could not deserialize JobQueue-Entry");
      }
    }

    public void Dispose() {
      _rabbitMqChannel?.Dispose();
      _rabbitMqConnection?.Dispose();
    }
  }
}
