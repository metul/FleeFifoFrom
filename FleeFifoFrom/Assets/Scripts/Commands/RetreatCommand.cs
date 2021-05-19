public class RetreatCommand : ResetCommand
{
    private Tile _tile, _battleFrontTile;

    public RetreatCommand(ulong issuerID, Tile battleFrontTile, Tile tile) : base(issuerID)
    {
        _tile = tile;
        _battleFrontTile = battleFrontTile;
    }

    public override void Execute()
    {
        base.Execute();
        var knight = _battleFrontTile.RemoveMeeple();
        _tile.SetMeeple(knight);
    }

    public override void Reverse()
    {
        base.Reverse();
        var knight = _tile.RemoveMeeple();
        _battleFrontTile.SetMeeple(knight);
    }

    public override void CheckFeasibility()
    {
        //for each tile in Row 5
        //If tile is empty
        //ActionPossible = true
        //break
    }
}
