using RabbitMQ.Client;
using Epic.Abstract;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using Epic.Models;
using Epic.Utilities;

namespace Epic.Rabbitmq;

public class RabbitmqConsumerService<TMessage> : BaseObservableService<TMessage>
{
    private readonly RabbitmqConfig _config;
    private readonly IDeserializer<TMessage> _deserializer;

    private IAsyncDisposable? _disposable;

    public RabbitmqConsumerService(IOptions<RabbitmqConfig> configurationOptions, IDeserializer<TMessage> deserializer)
    {
        _config = configurationOptions.Value;
        _deserializer = deserializer;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var connectionFactory = new ConnectionFactory
        {
            HostName = _config.Host,
            UserName = _config.Username,
            Password = _config.Password,
        };

        var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        var channel = await connection.CreateChannelAsync();

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, args) =>
        {
            var body = args.Body;
            var incomingMessage = _deserializer.Deserialize(body);

            var wrappedMessage = new Message<TMessage>(incomingMessage);
            PublishToAll(wrappedMessage);

            try
            {
                MessageStatus status = await wrappedMessage.Task;
                Console.WriteLine(status);
                if (status is not MessageStatus.Completed)
                {
                    throw new Exception("not completed");
                }

                await channel.BasicAckAsync(args.DeliveryTag, false, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                // Do metrics here
                // log error
            }
        };

        var tag = await channel.BasicConsumeAsync(_config.Queue, autoAck: false, consumer, cancellationToken);
        _disposable = new AnonymousAsyncDisposable(async () =>
        {
            await channel.BasicCancelAsync(tag);
            await channel.CloseAsync();
            await connection.CloseAsync();
        });
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return _disposable?.DisposeAsync().AsTask() ?? Task.CompletedTask;
    }
}

