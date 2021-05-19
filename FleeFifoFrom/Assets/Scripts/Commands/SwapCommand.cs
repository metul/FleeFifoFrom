public class SwapCommand : ActionCommand
{
    private Tile _tile1, _tile2;

    public SwapCommand(ulong issuerID, Tile tile1, Tile tile2) : base(issuerID)
    {
        _tile1 = tile1;
        _tile2 = tile2;
    }

    public override void Execute()
    {
        base.Execute();
        SwapTileMeeples(_tile1, _tile2);
    }

    public override void Reverse()
    {
        base.Reverse();
        SwapTileMeeples(_tile2, _tile1);
    }

    private void SwapTileMeeples(Tile tile1, Tile tile2)
    {
        var meeple1 = tile1.RemoveMeeple();
        var meeple2 = tile2.RemoveMeeple();
        tile1.SetMeeple(meeple2);
        tile2.SetMeeple(meeple1);
    }

    public override void CheckFeasibility()
    {
        //TODO Step 1: Start loop from tile 1
        if (piece.exists && !piece.injured)
        {
                //TODO: Can we come up with a smart data structure for easy neighbors
                //Tile.CheckNeighbors();
                //We want at least one neighbour that exists and is not injured
                //Break if yes, since action is possible
        }
        else (move to next tile)

        //TODO: end loop
    }

}
