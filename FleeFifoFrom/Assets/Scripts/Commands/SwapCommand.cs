public class SwapCommand : ActionCommand
{
    private DMeeple? _first;
    private DMeeple? _second;

    public SwapCommand(ulong issuerID, DMeeple? first, DMeeple? second) : base(issuerID)
    {
        _first = first;
        _second = second;
    }

    public override void Execute()
    {
        base.Execute();
        SwapTileMeeples(_first, _second);
    }

    public override void Reverse()
    {
        base.Reverse();
        SwapTileMeeples(_second, _first);
    }

    private void SwapTileMeeples(DMeeple first, DMeeple second)
    {
        DPosition tmp = first.Position.Current;
        first.Position.Current = second.Position.Current;
        second.Position.Current = tmp;
    }

    public override bool IsFeasibile()
    {
        return (
            _first != null && _second != null &&
            _first.IsHealthy() && _second.IsHealthy() &&
            _first.Position.Current.Neighbors(_second.Position.Current)
        );
    }
}
