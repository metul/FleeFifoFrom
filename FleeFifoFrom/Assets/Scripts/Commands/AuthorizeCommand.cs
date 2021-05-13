public class AuthorizeCommand : ActionCommand
{
    private Tile _tile;
    private Meeple _meeple;

    public AuthorizeCommand(ulong issuerID, Tile tile) : base(issuerID)
    {
        _tile = tile;
    }

    public override void Execute()
    {   
        base.Execute(); 
        var meeple = _tile.RemoveMeeple();

        // TODO Authorize: store away piece instead of destroy
        // S.R. Should we use a GameCommand class that contains helper commands
        // TODO: Notes for StoreAway commmand. Will need to redirect piece to correct owner
        Destroy(meeple.gameObject);
    }

    public override void Reverse()
    {
        base.Reverse();
        // TODO
    }
}
