namespace Epic.Abstract.Functions.Async;

public interface IAsyncFlatMapFunction<TFrom, TInto>
{
    Task<IList<TInto>> FlatMapAsync(TFrom data);
}
