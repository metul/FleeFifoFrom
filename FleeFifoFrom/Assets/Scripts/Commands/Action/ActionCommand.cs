public abstract class ActionCommand : Command
{
    protected DActionPosition.TileId _actionId;
    protected DWorker? _worker;
    protected DPlayer _player;

    public ActionCommand(ulong issuerID, DPlayer player, DWorker worker) : base(issuerID)
    {
        _player = player;
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
            _worker.UnConsume(_player.Id);
            GameState.Instance.TurnActionCount.Current--;
        }
    }

    public override bool IsFeasible()
    {
        return GameState.Instance.TurnType == GameState.TurnTypes.ActionTurn;
    }
}
