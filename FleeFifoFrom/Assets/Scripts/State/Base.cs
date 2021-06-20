/// Represents the parent for all data classes.
/// Data classes hold logical information for game objects.
public class DObject
{
    private static ushort ID_CURSOR = 0;

    public ushort ID { get => _id; private set => _id = value; }
    private ushort _id;

    public DObject(ushort id)
    {
        ID = id;
    }

    public DObject() // MARK: Is non-empty default constructor problematic for serialization?
    {
        ID = ID_CURSOR;
        ID_CURSOR++;
        ObjectManager.Instance.Register(ID, this);
    }
}