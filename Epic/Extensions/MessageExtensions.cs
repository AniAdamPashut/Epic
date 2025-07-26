using Epic.Abstract.Functions;
using Epic.Models;
using Epic.Models.Contexts;

namespace Epic.Extensions;

public static class MessageExtensions
{
    public static Message<T> Compose<T>(this Message<T> message, params List<Message<T>> messages)
    {
        var newContext = new MultiContext(Guid.NewGuid(), [.. messages.Append(message).Select(msg => msg.Context)]);
        return new Message<T>(message.Value, newContext);
    }

    public static IList<Message<T1>> BindContexts<T, T1>(this Message<T> message, Func<T, IList<T1>> flatMap)
    {
        var newValues = flatMap(message.Value);
        var sharedContext = new SharedContext(Guid.NewGuid(), message.Context, newValues.Count);

        return newValues.Select(val => new Message<T1>(val, sharedContext)).ToList();
    }

    public static async Task<IList<Message<T1>>> BindContexts<T, T1>(this Message<T> message,  Func<T, Task<IList<T1>>> flatMapFunction)
    {
        var newValues = await flatMapFunction(message.Value);
        var sharedContext = new SharedContext(Guid.NewGuid(), message.Context, newValues.Count);

        return [.. newValues.Select(val => new Message<T1>(val, sharedContext))];
    }
}
