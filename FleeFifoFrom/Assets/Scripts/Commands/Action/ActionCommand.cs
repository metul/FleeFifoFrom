using UnityEngine;
using MLAPI.Serialization;

public abstract class ActionCommand : Command, INetworkSerializable
{
    protected DActionPosition.TileId _actionId;
    protected DWorker? _worker;
    protected DPlayer.ID _playerId;

    // Default constructor needed for serialization
    public ActionCommand() : base() { }

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
        serializer.Serialize(ref _actionId);

        bool isWorkerNull = true;
        if (!serializer.IsReading)
            isWorkerNull = (_worker == null);

        serializer.Serialize(ref isWorkerNull);

        if (!isWorkerNull)
        {
            ushort workerID = ushort.MaxValue;
            if (!serializer.IsReading)
                workerID = _worker.ID;

            serializer.Serialize(ref workerID);

            if (serializer.IsReading)
                _worker = (DWorker)ObjectManager.Instance.Request(workerID);
        }

        serializer.Serialize(ref _playerId);
    }
}
