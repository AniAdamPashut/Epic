using System.Text;
using Epic.Core.Abstract;

namespace Epic.Core.Serialization;

public class UTF8Serializer : IDeserializer<string>
{
    public string Deserialize(ReadOnlyMemory<byte> bytes)
    {
        return Encoding.UTF8.GetString(bytes.Span);
    }
}
