namespace Epic.Models;

public record MessageStatus(Context Context)
{
    public sealed record Completed(Context context) : MessageStatus(context);
    public sealed record Filtered(string why, Context context) : MessageStatus(context);
    public sealed record Dropped(string why, Context context) : MessageStatus(context);
}