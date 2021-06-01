public class PoachCommand: ResetCommand
{
    private DPlayer.ID _player;
    private DWorker _worker;
    private DPlayer.ID _controlledBy;
    private DActionPosition.TileId _previousTile;
    
    public PoachCommand(ulong issuerID, DPlayer.ID player, DWorker worker) : base(issuerID)
    {
        _worker = worker;
        _controlledBy = worker.ControlledBy;
        _player = player;
        if (worker.Position.Current.Tile != null) _previousTile = (DActionPosition.TileId) worker.Position.Current.Tile;
    }
    
    public override void Execute()
    {
        base.Execute();
        _worker.Poach(_player);

    }

    public override void Reverse()
    {
        base.Reverse();
        _worker.Return(_controlledBy, _previousTile);

    }

    public override bool IsFeasibile()
    {
        return _worker.Position.Current.IsActionTile && _worker.Owner != _player;
    }
}