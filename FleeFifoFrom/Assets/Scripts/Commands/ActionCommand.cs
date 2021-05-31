public abstract class ActionCommand : Command
{
    protected DActionPosition.TileId _actionId;
    protected DWorker _worker;
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
        _worker.Consume(_actionId);
    }

    public override void Reverse()
    {
        _worker.UnConsume(_playerId);
    }
}
