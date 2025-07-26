using System.Diagnostics.Contracts;

namespace Epic.Models;

public abstract record Context(Guid UniqueId)
{
    private readonly TaskCompletionSource<MessageStatus> _taskCompletionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public Task<MessageStatus> ReportingTask => _taskCompletionSource.Task;

    public virtual void SetStatus(MessageStatus status)
    {
        _taskCompletionSource.SetResult(status);
    }
}