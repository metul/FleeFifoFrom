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
        Destroy(meeple.gameObject);
    }

    public override void Reverse()
    {
        base.Reverse();
        // TODO
    }
}
