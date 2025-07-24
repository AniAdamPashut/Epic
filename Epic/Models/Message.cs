namespace Epic.Models;

public record Message<T>(T Value, Context Context)
{
    public Task<MessageStatus> Task => Context._taskCompletionSource.Task;

    public void Drop(string reason)
    {
        Context.SetStatus(new MessageStatus.Dropped(reason, Context));
    }

    public void Filter(string reason)
    {
        Context.SetStatus(new MessageStatus.Filtered(reason, Context));
    }

    public void Acknowledge()
    {
        Context.SetStatus(new MessageStatus.Completed(Context));
    }
}