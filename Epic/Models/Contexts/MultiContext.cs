namespace Epic.Models;

public sealed record MultiContext(Guid UniqueId, IList<Context> Contexts) : Context(UniqueId)
{
    public override void SetStatus(MessageStatus status)
    {
        foreach (var context in Contexts)
        {
            context.SetStatus(status with {Context = context});
        }
    }
}
