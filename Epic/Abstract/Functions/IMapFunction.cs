namespace Epic.Abstract.Functions;

public interface IMapFunction<TFrom, TInto>
{
    TInto Map(TFrom data);
}
