namespace Epic.Core.Abstract;

public interface IDeserializer<T>
{
    T Deserialize(ReadOnlyMemory<byte> bytes);
}
