using System.Text;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Newtonsoft.Json;
using RabbitMQ.Client;
using VidloadShared.Models.Configuration;
using VidloadShared.Models.Jobs;

namespace VidloadPortal.Services {
  public class JobEnqueuer : IJobEnqueuer {
    private IModel _rabbitMqChannel;
    private IConnection _rabbitMqConnection;
    private readonly VidloadConfiguration _vidloadConfiguration;

    public JobEnqueuer(VidloadConfiguration vidloadConfiguration) {
      _vidloadConfiguration = vidloadConfiguration;
    }

    public void Open() {
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
    }

    public Task<Result> Enqueue(MediaMetadataJob mediaMetadataJob) {
      try {
        var serialized = JsonConvert.SerializeObject(mediaMetadataJob);
        var encoded = Encoding.UTF8.GetBytes(serialized);

        _rabbitMqChannel.BasicPublish(
          string.Empty,
          _vidloadConfiguration.JobQueueConfiguration.MediaMetadataJobQueueName,
          null,
          encoded
        );
        return Task.FromResult(Result.Success());
      } catch {
        return Task.FromResult(Result.Failure("Could not enqueue Job"));
      }
    }

    public Task<Result> Enqueue(MediaDownloadJob mediaDownloadJob) {
      try {
        var serialized = JsonConvert.SerializeObject(mediaDownloadJob);
        var encoded = Encoding.UTF8.GetBytes(serialized);

        _rabbitMqChannel.BasicPublish(
          string.Empty,
          _vidloadConfiguration.JobQueueConfiguration.MediaDownloadJobQueueName,
          null,
          encoded
        );
        return Task.FromResult(Result.Success());
      } catch {
        return Task.FromResult(Result.Failure("Could not enqueue Job"));
      }
    }

    public void Dispose() {
      _rabbitMqChannel?.Dispose();
      _rabbitMqConnection?.Dispose();
    }
  }
}
