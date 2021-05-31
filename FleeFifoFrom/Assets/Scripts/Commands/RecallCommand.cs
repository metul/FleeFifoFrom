using System.Collections.Generic;

public class RecallCommand: ResetCommand
{
    private DActionPosition.TileId _tileId;
    private List<DWorker> _workers;
    private List<DPlayer.ID> _controlling;

    public RecallCommand(ulong issuerID, DActionPosition.TileId tileId) : base(issuerID)
    {
        _tileId = tileId;
        _workers = GameStateUtils.AtTilePosition(GameState.Instance, new DActionPosition(tileId));
        _controlling = new List<DPlayer.ID>();

        foreach (var dWorker in _workers)
            _controlling.Add(dWorker.ControlledBy);
        
    }

    public override void Execute()
    {
        base.Execute();
        
        foreach (var dWorker in _workers)
            dWorker.Release();
        
    }

    public override void Reverse()
    {
        base.Reverse();    
        
        for (var i = _workers.Count - 1; i >= 0; i--)
            _workers[i].UnRelease(_controlling[i], _tileId);
    }

    public override bool IsFeasibile()
    {
        return true;
    }
}
