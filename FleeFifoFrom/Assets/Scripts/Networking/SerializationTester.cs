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
