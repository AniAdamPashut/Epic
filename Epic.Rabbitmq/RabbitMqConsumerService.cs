using RabbitMQ.Client;
using Epic.Abstract;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using Epic.Models;
using Epic.Utilities;
using Epic.Extensions;
using Microsoft.Extensions.Logging;
using Epic.Abstract.Serialization;

namespace Epic.Rabbitmq;

public class RabbitMqConsumerService<TMessage> : BaseObservableService<TMessage>
{
    private readonly RabbitMqConfig _config;
    private readonly IDeserializer<TMessage> _deserializer;
    private readonly ILogger<RabbitMqConsumerService<TMessage>> _logger;

    private IAsyncDisposable? _disposable;

    public RabbitMqConsumerService(
        IOptions<RabbitMqConfig> configurationOptions,
        IDeserializer<TMessage> deserializer,
        ILoggerFactory loggerFactory)
    {
        _config = configurationOptions.Value;
        _deserializer = deserializer;

        _logger = loggerFactory.CreateLogger<RabbitMqConsumerService<TMessage>>();
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting the RabbitMq Consumer Service");

        var connectionFactory = new ConnectionFactory
        {
            HostName = _config.Host,
            UserName = _config.Username,
            Password = _config.Password,
        };

        var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(_config.Queue);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, args) =>
        {
            var body = args.Body;
            var incomingMessage = _deserializer.Deserialize(body);

            var context = new RabbitMqContext(Guid.NewGuid(), args.DeliveryTag);
            var wrappedMessage = new Message<TMessage>(incomingMessage, context);

            PublishToAll(wrappedMessage);

            try
            {
                MessageStatus status = await wrappedMessage.Task;
                RabbitMqContext rabbitmqContext = status.GetContext<RabbitMqContext>();

                await channel.BasicAckAsync(rabbitmqContext.DeliveryTag, false, cancellationToken);
                _logger.LogInformation("Acked the message \"{incomingMessage}\"", incomingMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Encountered an exception");
            }
        };

        var tag = await channel.BasicConsumeAsync(_config.Queue, autoAck: false, consumer, cancellationToken);

        _disposable = new AnonymousAsyncDisposable(async () =>
        {
            await channel.BasicCancelAsync(tag);
            await channel.CloseAsync();
            await connection.CloseAsync();

            CloseAll();
        });

        _logger.LogInformation("Started the RabbitMq Consumer Service");
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping the RabbitMq Consumer Service");
        return _disposable?.DisposeAsync().AsTask() ?? Task.CompletedTask;
    }
}

