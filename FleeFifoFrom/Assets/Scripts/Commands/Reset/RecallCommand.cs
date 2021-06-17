using System.Collections.Generic;

public class RecallCommand: ResetCommand
{
    private DPlayer.ID _player;
    private DActionPosition.TileId _tileId;
    private List<DWorker> _workers;
    private List<DPlayer.ID> _controlling;

    public RecallCommand(ulong issuerID, DPlayer.ID player, DActionPosition.TileId tileId) : base(issuerID)
    {
        _player = player;
        _tileId = tileId;
        _workers = GameStateUtils.AtTilePosition(GameState.Instance, new DActionPosition(tileId));
        _controlling = new List<DPlayer.ID>();

        foreach (var dWorker in _workers)
            _controlling.Add(dWorker.ControlledBy);
        
    }

    public override void Execute()
    {
        base.Execute();


        //If you cooperate at least once, get honor
        bool cooperate = false;

        foreach (var dWorker in _workers)
        {
            dWorker.Release();
            if (dWorker.ControlledBy != _player) cooperate = true;
        }
           
        if(cooperate) GameState.Instance.PlayerById(_player)?.Honor.Earn();

    }

    public override void Reverse()
    {
        base.Reverse();

        bool cooperate = false;

        for (var i = _workers.Count - 1; i >= 0; i--)
        {
            if (_workers[i].ControlledBy != _player) cooperate = true;
            _workers[i].Return(_controlling[i], _tileId);            
        }

        //Take back honor if the player had cooperated
        if (cooperate) GameState.Instance.PlayerById(_player)?.Honor.Lose();
    }

    public override bool IsFeasible()
    {

        //TODO: I think we also need && _worker.Position.Current.IsActionTile
        //Only allow if the tile has at least one worker
        return base.IsFeasible() && _workers.ToArray().Length >= 1;


    }
}
