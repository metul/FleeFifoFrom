using MLAPI.Serialization;

public class MoveVillagerCommand : ResetCommand, INetworkSerializable
{
    private DMeeple _meeple;
    private DPosition _to;
    private DPosition _from;
    private DPlayer _player;

    // Default constructor needed for serialization
    public MoveVillagerCommand() : base() { }
    public MoveVillagerCommand(ulong issuerID, DPlayer player, DMeeple meeple, DPosition to) : base(issuerID)
    {
        _player = player;
        _meeple = meeple;
        _to = to;
        _from = meeple.Position.Current;
        _freeCommand = true;
    }

    public override void Execute()
    {
        base.Execute();

        //i.e. lower honor if priority broken, then move piece as usual  
        if (!GameState.Instance.CheckPriority(_meeple))
            _player.Honor.Lose();

        _meeple.Position.Current = _to;
    }

    public override void Reverse()
    {
        base.Reverse();
        _meeple.Position.Current = _from;

        //i.e. raise honor retroactively if priority would have been broken 
        if (!GameState.Instance.CheckPriority(_meeple))
            _player.Honor.Earn();

    }

    public override bool IsFeasible()
    {
        return base.IsFeasible()
          && GameState.Instance.IsEmpty(_to)
          && _meeple.Position.Current.CanMoveTo(_to);
    }

    public override void NetworkSerialize(NetworkSerializer serializer)
    {
        base.NetworkSerialize(serializer);

        ushort meepleID = ushort.MaxValue;
        if (!serializer.IsReading)
            meepleID = _meeple.ID;

        serializer.Serialize(ref meepleID);

        if (serializer.IsReading)
            _meeple = (DMeeple)RegistryManager.Instance.Request(meepleID); // TODO (metul): Do we need further type casting down the line (e.g. villager)?

        if (serializer.IsReading)
        {
            _to = new DPosition();
            _from = new DPosition();
        }

        _to.NetworkSerialize(serializer);
        _from.NetworkSerialize(serializer);

        DPlayer.ID playerID = DPlayer.ID.Black;
        if (!serializer.IsReading)
            playerID = _player.Id;

        serializer.Serialize(ref playerID);

        if (serializer.IsReading)
            _player = RegistryManager.Instance.Request(playerID);
    }
}