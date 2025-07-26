namespace Epic.Models;

public abstract record MessageStatus(Context Context)
{
    public sealed record Completed(Context Context) : MessageStatus(Context);
    public sealed record Filtered(string Why, Context Context) : MessageStatus(Context);
    public sealed record Dropped(string Why, Context Context) : MessageStatus(Context);
}
