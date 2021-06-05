using UnityEngine;

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

    public void CheckWorker()
    {
        // TODO Step 1: Check that there are enough workers to use an action
        // TODO: Should this be a subsection of execute instead?
        // -> if(_worker.Available)
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
}
