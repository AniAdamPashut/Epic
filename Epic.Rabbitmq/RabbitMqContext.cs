using Epic.Models;

namespace Epic.Rabbitmq;

public sealed record class RabbitMqContext(Guid UniqueId, ulong DeliveryTag) : Context(UniqueId);