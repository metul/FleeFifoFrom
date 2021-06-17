using MLAPI.Serialization;
/// Reprsents the parent for all data classes.
/// Data classes hold logical information for game objects.
public class DObject : INetworkSerializable
{
    private static ushort ID_CURSOR = 0;

    public ushort ID { get => _id; private set => _id = value; }
    private ushort _id;

    public DObject(ushort id)
    {
        ID = id;
    }

    public DObject()
    {
        ID = ID_CURSOR;
        ID_CURSOR++;
    }

    public virtual void NetworkSerialize(NetworkSerializer serializer)
    {
        serializer.Serialize(ref _id);
    }
}