namespace Epic.Models;

public record MessageStatus
{
    public sealed record Completed : MessageStatus;
    public sealed record Filtered(string why) : MessageStatus;
    public sealed record Dropped(string why) : MessageStatus;
}