public class ReviveCommand : ActionCommand
{
    //private Tile _tile;
    private readonly DMeeple _meeple;
    private readonly DPosition _position;
    // private Meeple.State _originalState; // MARK: Redundant, could be replaced with Meeple.State.Injured

    public ReviveCommand(ulong issuerID, DPlayer.ID playerId, DWorker worker, DMeeple meeple) : base(issuerID, playerId, worker)
    {
        _actionId = DActionPosition.TileId.Revive;
        _meeple = meeple;
        _position = meeple.Position.Current;
    }


    public override void Execute()
    {
        base.Execute();
 
            //Heal the meeple
            ((DVillager) _meeple).Heal();

            //Distribute Honor

            // TODO: who gets honor with player != token owner
            //Remove hard coded 1
            GameState.Instance.PlayerById(_worker.ControlledBy)?.Honor.Earn();
        
    }

    public override void Reverse()
    {
        base.Reverse();
        // _tile.Meeple.CurrentState = _originalState;
    }

    public override bool IsFeasibile()
    {
        return (_meeple.IsInjured());
    }
}
