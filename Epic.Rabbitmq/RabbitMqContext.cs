using Epic.Models;

namespace Epic.Rabbitmq;

public sealed record RabbitMqContext(Guid UniqueId, ulong DeliveryTag) : Context(UniqueId);