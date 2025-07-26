using System.Text;
using Epic.Abstract.Serialization;

namespace Epic.Serialization;

public class UTF8Serializer : IDeserializer<string>, ISerializer<string>
{
    public string Deserialize(ReadOnlyMemory<byte> bytes)
    {
        return Encoding.UTF8.GetString(bytes.Span);
    }

    public ReadOnlyMemory<byte> Serialize(string data)
    {
        return Encoding.UTF8.GetBytes(data);
    }
}
