using MLAPI.Logging;
using MLAPI.Serialization;

public class SerializationTester : INetworkSerializable
{
    protected int _myInt;

    // Default constructor needed for serialization
    public SerializationTester() { }

    public SerializationTester(int value)
    {
        _myInt = value;
    }

    public virtual void Execute()
    {
        NetworkLog.LogInfoServer($"SerializationTester with value {_myInt} executed");
    }

    public virtual void NetworkSerialize(NetworkSerializer serializer)
    {
        serializer.Serialize(ref _myInt);
    }
}

public class SerializationTesterSubclassA : SerializationTester, INetworkSerializable
{
    private string _myString;

    // Default constructor needed for serialization
    public SerializationTesterSubclassA() : base() { }

    public SerializationTesterSubclassA(int valueInt, string valueString) : base(valueInt)
    {
        _myString = valueString;
    }

    public override void Execute()
    {
        NetworkLog.LogInfoServer($"SerializationTesterSubclass with value {_myInt} / {_myString} executed");
    }

    public override void NetworkSerialize(NetworkSerializer serializer)
    {
        base.NetworkSerialize(serializer);
        serializer.Serialize(ref _myString);
    }
}

public class SerializationTesterSubclassB : SerializationTester, INetworkSerializable
{
    private string _myString;

    // Default constructor needed for serialization
    public SerializationTesterSubclassB() : base() { }

    public SerializationTesterSubclassB(int valueInt, string valueString) : base(valueInt)
    {
        _myString = valueString;
    }

    public override void Execute()
    {
        NetworkLog.LogInfoServer($"SerializationTesterSubclass with value {_myInt} / {_myString} executed");
    }

    public override void NetworkSerialize(NetworkSerializer serializer)
    {
        base.NetworkSerialize(serializer);
        serializer.Serialize(ref _myString);
    }
}

public class SerializationTesterSubclassAuthorize : SerializationTester, INetworkSerializable
{
    private DMeeple _meeple;
    private DPosition _position;

    // Default constructor needed for serialization
    public SerializationTesterSubclassAuthorize() : base() { }

    public SerializationTesterSubclassAuthorize(int valueInt, DMeeple meeple) : base(valueInt)
    {
        _meeple = meeple;
        _position = meeple.Position.Current;
    }

    public override void Execute()
    {
        NetworkLog.LogInfoServer($"Executed SerializationTesterSubclassAuthorize with value {_myInt}, " +
            $"meeple ({_meeple.ID} / {_meeple.Position.Current} / {_meeple.State}) and position {_position}");
    }

    public override void NetworkSerialize(NetworkSerializer serializer)
    {
        base.NetworkSerialize(serializer);

        ushort meepleID = ushort.MaxValue;
        if (!serializer.IsReading)
            meepleID = _meeple.ID;

        serializer.Serialize(ref meepleID);

        if (serializer.IsReading)
            _meeple = (DMeeple)ObjectManager.Instance.Request(meepleID); // TODO: Do we need further type casting down the line (e.g. villager)?

        if (serializer.IsReading)
            _position = new DPosition();

        _position.NetworkSerialize(serializer);
    }
}