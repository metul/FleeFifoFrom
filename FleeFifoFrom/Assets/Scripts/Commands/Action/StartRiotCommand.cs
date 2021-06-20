using UnityEngine;
using MLAPI.Serialization;

public class StartRiotCommand : ActionCommand, INetworkSerializable
{
    private DMeeple _meeple;

    // Default constructor needed for serialization
    public StartRiotCommand() : base() { }

    public StartRiotCommand(
      ulong issuerID,
      DPlayer.ID playerId,
      DWorker worker,
      DMeeple meeple
    ) : base(issuerID, playerId, worker)
    {
        _actionId = DActionPosition.TileId.Riot;
        _meeple = meeple;
    }

    public override void Execute()
    {
        base.Execute();
        GameState.Instance.PlayerById(_worker.Owner)?.Honor.Lose();
    }

    public override void Reverse()
    {
        base.Reverse();
        GameState.Instance.PlayerById(_worker.Owner)?.Honor.Earn();
    }

    public override bool IsFeasible()
    {
        return base.IsFeasible() && _meeple.GetType() == typeof(DKnight);
    }

    public override void NetworkSerialize(NetworkSerializer serializer)
    {
        base.NetworkSerialize(serializer);
        _meeple.NetworkSerialize(serializer);
    }
}