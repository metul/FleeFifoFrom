using MLAPI.Serialization;

public class ReprioritizeCommand : ResetCommand, INetworkSerializable
{
    private string _prioType;
    private DPrio _prio;
    private bool _increase;

    // Default constructor needed for serialization
    public ReprioritizeCommand() : base() { }

    public ReprioritizeCommand(ulong issuerID, string prioType, bool increase) : base(issuerID)
    {
        _prioType = prioType;
        _prio = GameState.Instance.Priorities[PriorityTileManager.STRING_TYPE_MAPPING[_prioType]];
        _increase = increase;
    }

    public override void Execute()
    {
        base.Execute();
        if(_increase)
            _prio.Increase();
        else
            _prio.Decrease();
    }

    public override void Reverse()
    {
        base.Reverse();
        if(_increase)
            _prio.Decrease();
        else
            _prio.Increase();
    }

    public override bool IsFeasible()
    {
        return _increase ? _prio.IsIncreasable : _prio.IsDecreasable;
    }

    public override void NetworkSerialize(NetworkSerializer serializer)
    {
        base.NetworkSerialize(serializer);

        serializer.Serialize(ref _prioType);

        if (serializer.IsReading)
            _prio = GameState.Instance.Priorities[PriorityTileManager.STRING_TYPE_MAPPING[_prioType]];

        serializer.Serialize(ref _increase);
    }
}
