using Epic.Abstract.Functions.Async;

namespace Epic.Playground.Processors;

public class DelayByLengthAsyncMapFunction : IAsyncMapFunction<(string Value, Guid Guid), (string Value, Guid Guid)>
{
    public async Task<(string Value, Guid Guid)> MapAsync((string Value, Guid Guid) data)
    {
        await Task.Delay(data.Value.Length * 1000);
        return data;
    }
}
