using System.Collections.Generic;
using MLAPI.Serialization;

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

        //S.R.: Can the following conditional be added
        //TODO: If exists(_worker.Owner != _player), Honor ++
        //i.e. if you reset at least one opponent worker, then gain honor


        foreach (var dWorker in _workers)
            dWorker.Release();
        
    }

    public override void Reverse()
    {
        base.Reverse();    
        
        for (var i = _workers.Count - 1; i >= 0; i--)
            _workers[i].Return(_controlling[i], _tileId);
    }

    public override bool IsFeasible()
    {

        //TODO: I think we also need && _worker.Position.Current.IsActionTile
        //TODO: i.e. the tile needs to have at least one worker
        return base.IsFeasible();
        
    }

    public override void NetworkSerialize(NetworkSerializer serializer)
    {
        base.NetworkSerialize(serializer);
        serializer.Serialize<DActionPosition.TileId>(ref _tileId);
        // TODO: Serialize List<DWorker> _workers
        //foreach (DWorker worker in _workers)
        //    worker.NetworkSerialize(serializer);
        // TODO: Serialize List<DPlayer.ID> _controlling
    }
}
