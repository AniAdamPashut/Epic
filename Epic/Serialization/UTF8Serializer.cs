using System.Text;
using Epic.Abstract;

namespace Epic.Serialization;

public class UTF8Serializer : IDeserializer<string>
{
    public string Deserialize(ReadOnlyMemory<byte> bytes)
    {
        return Encoding.UTF8.GetString(bytes.Span);
    }
}
