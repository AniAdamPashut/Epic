namespace Epic.Models.Contexts;

public record SharedContext(Guid UniqueId, Context InnerContext, int TotalSharing) : Context(UniqueId)
{
    private int _calledCount = 0;

    public override void SetStatus(MessageStatus status)
    {
        if (Interlocked.Increment(ref _calledCount) == TotalSharing)
        {
            InnerContext.SetStatus(status);
        }
    }
}
