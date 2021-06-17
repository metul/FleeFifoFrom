using UnityEngine;
using MLAPI.Serialization;

public abstract class ActionCommand : Command
{
    protected DActionPosition.TileId _actionId;
    protected DWorker? _worker;
    protected DPlayer.ID _playerId;

    public ActionCommand(ulong issuerID, DPlayer.ID playerId, DWorker worker) : base(issuerID)
    {
        _playerId = playerId;
        _worker = worker;
    }

    public override void Execute()
    {
        if (_worker != null)
        {
            _worker.Consume(_actionId);
            GameState.Instance.TurnActionCount.Current++;
        }
    }

    public override void Reverse()
    {
        if (_worker != null)
        {
            _worker.UnConsume(_playerId);
            GameState.Instance.TurnActionCount.Current--;
        }
    }

    public override bool IsFeasible()
    {
        return GameState.Instance.TurnType == GameState.TurnTypes.ActionTurn;
    }

    public override void NetworkSerialize(NetworkSerializer serializer)
    {
        base.NetworkSerialize(serializer);
        serializer.Serialize<DActionPosition.TileId>(ref _actionId);
        _worker.NetworkSerialize(serializer);
        serializer.Serialize<DPlayer.ID>(ref _playerId);
    }
}
