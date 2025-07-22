namespace Epic.Rabbitmq;

public record class RabbitmqConfig
{
    public required string Host { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string Queue { get; init; }
}
