public class ReprioritizeCommand : ResetCommand
{
    private Tile _tile;
    private Meeple.State _originalState;

    public ReprioritizeCommand(ulong issuerID, Tile tile) : base(issuerID)
    {
        _tile = tile;
    }

    public override void Execute()
    {
        base.Execute();
        _originalState = _tile.Meeple.CurrentState;
        _tile.Meeple.CurrentState = _tile.Meeple.CurrentState == Meeple.State.Default
            ? Meeple.State.Tapped
            : Meeple.State.Default;
    }

    public override void Reverse()
    {
        base.Reverse();
        _tile.Meeple.CurrentState = _originalState;
    }
}
