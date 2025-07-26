using Epic.Abstract.Functions;

namespace Epic.Playground.Processors;

public class MultiplyStringMapFunction : IMapFunction<(string Value, Guid Guid), (string Value, Guid Guid)>
{
    public (string Value, Guid Guid) Map((string Value, Guid Guid) data)
    {
        return data with { Value = data.Value + data.Value };
    }
}
