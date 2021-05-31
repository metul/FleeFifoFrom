public class ReviveCommand : ActionCommand
{
    private Tile _tile;
    // private Meeple.State _originalState; // MARK: Redundant, could be replaced with Meeple.State.Injured

    public ReviveCommand(ulong issuerID, DPlayer.ID playerId, DWorker worker, Tile tile) : base(issuerID, playerId, worker)
    {
        _tile = tile;
    }

    public override void Execute()
    {
        base.Execute();
        // _originalState = _tile.Meeple.CurrentState;
        // _tile.Meeple.CurrentState = Meeple.State.Default;
    }

    public override void Reverse()
    {
        base.Reverse();
        // _tile.Meeple.CurrentState = _originalState;
    }

    public override bool IsFeasibile()
    {
        //TODO Step 1: Start loop from tile 1
        return false;
        /*psuedo
        if (piece.exists)
        {
            if (piece.injured)
            {   
                //Atleast one injured piece found
                this.ActionPossible = true;
                break;
            }

        }
        else (move to next tile)

        //TODO: end loop. If no injuries found, stay as false
        */
    }
}
