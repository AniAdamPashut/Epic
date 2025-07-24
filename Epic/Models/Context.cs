namespace Epic.Models;

public abstract record Context(Guid UniqueId)
{
    internal readonly TaskCompletionSource<MessageStatus> _taskCompletionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public virtual void SetStatus(MessageStatus status)
    {
        _taskCompletionSource.SetResult(status);
    }
}