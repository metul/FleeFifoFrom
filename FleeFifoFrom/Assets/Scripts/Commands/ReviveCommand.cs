public class ReviveCommand : ActionCommand
{
    private Tile _tile;
    private Meeple.State _originalState; // MARK: Redundant, could be replaced with Meeple.State.Injured

    public ReviveCommand(ulong issuerID, Tile tile) : base(issuerID)
    {
        _tile = tile;
    }

    public override void Execute()
    {
        base.Execute();
        _originalState = _tile.Meeple.CurrentState;
        _tile.Meeple.CurrentState = Meeple.State.Default;
    }

    public override void Reverse()
    {
        base.Reverse();
        _tile.Meeple.CurrentState = _originalState;
    }

    public override void CheckFeasibility()
    {
        //TODO Step 1: Start loop from tile 1
        this.ActionPossible = false;
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
