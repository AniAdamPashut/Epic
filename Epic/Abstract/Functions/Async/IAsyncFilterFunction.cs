namespace Epic.Abstract.Functions.Async;

public interface IAsyncFilterFunction<TData>
{
    string Reason { get; }

    Task<bool> ShouldFilterAsync(TData data);
}
