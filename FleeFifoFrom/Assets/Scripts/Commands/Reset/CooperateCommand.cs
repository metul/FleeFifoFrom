using MLAPI.Serialization;

public class CooperateCommand: ResetCommand, INetworkSerializable
{
    private DPlayer.ID _player;
    private DWorker _worker;
    private DPlayer.ID _controlledBy;
    private DActionPosition.TileId _previousTile;

    //TODO: Maybe deprecate, by consolidating functionality into Recall

    // Default constructor needed for serialization
    public CooperateCommand() : base() { }

    public CooperateCommand(ulong issuerID, DPlayer.ID player, DWorker worker) : base(issuerID)
    {
        _worker = worker;
        _controlledBy = worker.ControlledBy;
        _player = player;
        if (worker.Position.Current.Tile != null) _previousTile = (DActionPosition.TileId) worker.Position.Current.Tile;
    }

    public override void Execute()
    {
        base.Execute();
        _worker.Release();
    }

    public override void Reverse()
    {
        base.Reverse();
        _worker.Return(_controlledBy, _previousTile);
    }

    public override bool IsFeasible()
    {
        return base.IsFeasible() && _worker.Position.Current.IsActionTile && _worker.Owner != _player;
    }

    public override void NetworkSerialize(NetworkSerializer serializer)
    {
        base.NetworkSerialize(serializer);
        serializer.Serialize(ref _player);

        ushort workerID = ushort.MaxValue;
        if (!serializer.IsReading)
            workerID = _worker.ID;

        serializer.Serialize(ref workerID);

        if (serializer.IsReading)
            _worker = (DWorker)RegistryManager.Instance.Request(workerID);

        serializer.Serialize(ref _controlledBy);
        serializer.Serialize(ref _previousTile);
    }
}
