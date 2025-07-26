namespace Epic.Abstract.Functions;

public interface IFlatMapFunction<TFrom, TInto>
{
    IList<TInto> FlatMap(TFrom data);
}
