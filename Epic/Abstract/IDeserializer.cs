namespace Epic.Abstract;

public interface IDeserializer<T>
{
    T Deserialize(ReadOnlyMemory<byte> bytes);
}
