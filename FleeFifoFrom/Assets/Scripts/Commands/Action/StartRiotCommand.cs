using MLAPI.Serialization;

public class StartRiotCommand : ActionCommand, INetworkSerializable
{
    private DMeeple _meeple;

    // Default constructor needed for serialization
    public StartRiotCommand() : base() { }
    public StartRiotCommand(
      ulong issuerID,
      DPlayer player,
      DWorker worker,
      DMeeple meeple
    ) : base(issuerID, player, worker)
    {
        _actionId = DActionPosition.TileId.Riot;
        _meeple = meeple;
    }

    public override void Execute()
    {
        base.Execute();
        ((DKnight)_meeple).IsRioting.Current = true;
        GameState.Instance.PlayerById(_worker.Owner)?.Honor.Lose();
    }

    public override void Reverse()
    {
        base.Reverse();
        ((DKnight)_meeple).IsRioting.Current = false;
        GameState.Instance.PlayerById(_worker.Owner)?.Honor.Earn();
    }

    public override bool IsFeasible()
    {
        return base.IsFeasible() && _meeple.GetType() == typeof(DKnight);
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
    }
}