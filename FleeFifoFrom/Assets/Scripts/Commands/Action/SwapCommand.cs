public class SwapCommand : ActionCommand
{
    private DMeeple? _first;
    private DMeeple? _second;
    
    public SwapCommand(ulong issuerID, DPlayer player, DWorker worker, DMeeple? first, DMeeple? second) : base(issuerID, player, worker)
    {
        _actionId = DActionPosition.TileId.Swap;
        _worker = worker;
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
        DPosition firstPos = first.Position.Current;
        DPosition secondPos = second.Position.Current;
        first.Position.Current = second.Position.Current = null;
        first.Position.Current = secondPos;
        second.Position.Current = firstPos;
    }

    public override bool IsFeasible()
    {
        return base.IsFeasible() &&
            (
                _first != null && _second != null &&
                _first.IsHealthy() && _second.IsHealthy() &&
                _first.Position.Current.Neighbors(_second.Position.Current)
            );

        //TODO: The swap priority command should always be feasible after base
    }
}
