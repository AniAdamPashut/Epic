using Epic.Abstract.Functions;

namespace Epic.Playground.Processors;

public class SplitBySpaceFlatMapFunction : IFlatMapFunction<string, (string Value, Guid Guid)>
{
    public IList<(string Value, Guid Guid)> FlatMap(string str)
    {
        var values = str.Split(' ');
        var guid = Guid.NewGuid();
        return values.Select(val => (val, guid)).ToList();
    }
}
