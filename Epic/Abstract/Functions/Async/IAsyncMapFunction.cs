namespace Epic.Abstract.Functions.Async;

public interface IAsyncMapFunction<TFrom, TInto>
{
    Task<TInto> MapAsync(TFrom data);
}
