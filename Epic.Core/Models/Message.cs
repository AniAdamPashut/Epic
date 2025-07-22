namespace Epic.Core.Models;

public record Message<T>(T Value)
{
    internal readonly TaskCompletionSource<MessageStatus> _taskCompletionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);

    internal Message(T val, TaskCompletionSource<MessageStatus> taskCompletionSource)
        : this(val)
    {
        _taskCompletionSource = taskCompletionSource;
    }

    public Task<MessageStatus> Task => _taskCompletionSource.Task;

    public void Drop(string reason)
    {
        _taskCompletionSource.SetResult(new MessageStatus.Dropped(reason));
    }

    public void Filter(string reason)
    {
        _taskCompletionSource.SetResult(new MessageStatus.Filtered(reason));
    }

    public void Acknowledge()
    {
        _taskCompletionSource.SetResult(new MessageStatus.Completed());
    }
}