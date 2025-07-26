namespace Epic.Abstract.Serialization;

public interface IDeserializer<TData>
{
    TData Deserialize(ReadOnlyMemory<byte> bytes);
}
