namespace Epic.Abstract.Functions;

public interface IFilterFunction<TData>
{
    string Reason { get; }

    bool ShouldFilter(TData data);
}
