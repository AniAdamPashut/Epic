namespace Epic.Abstract.Serialization;

public interface ISerializer<TData>
{
    ReadOnlyMemory<byte> Serialize(TData data);
}
