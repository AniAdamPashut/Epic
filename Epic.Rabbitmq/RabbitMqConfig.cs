namespace Epic.Rabbitmq;

public record RabbitMqConfig
{
    public required string Host { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string Queue { get; init; }
}
