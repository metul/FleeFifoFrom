using MLAPI.Serialization;

public class SwapCommand : ActionCommand, INetworkSerializable
{
    private DMeeple? _first;
    private DMeeple? _second;

    //private MeeplePriority? _firstPriority;
    //private MeeplePriority? _secondPriority;

    // Default constructor needed for serialization
    public SwapCommand() : base() { }

    public SwapCommand(ulong issuerID, DPlayer.ID playerId, DWorker worker, DMeeple? first, DMeeple? second) : base(issuerID, playerId, worker)
    {
        _actionId = DActionPosition.TileId.Swap;
        _worker = worker;
        _first = first;
        _second = second;
    }

    //TODO: Thinking we have two constructor definitions for the swap command
    /*
    public SwapCommand(ulong issuerID, DPlayer.ID playerId, DWorker worker, MeeplePriority? firstPriority, MeeplePriority? secondPriority) : base(issuerID, playerId, worker)
    {
        _actionId = DActionPosition.TileId.Swap;
        _worker = worker;
        _firstPriority = first;
        _secondPriority = second;
    }
    */


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

    //TODO: SwapPriority version
    /*
     *     private void SwapPriority(MeeplePriority first, MeeplePriority second)
    {
        DPosition firstPos = first.Position.Current;
        DPosition secondPos = second.Position.Current;
        first.Position.Current = second.Position.Current = null;
        first.Position.Current = secondPos;
        second.Position.Current = firstPos;
    }
    */

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

    public override void NetworkSerialize(NetworkSerializer serializer)
    {
        base.NetworkSerialize(serializer);
        _first.NetworkSerialize(serializer); // TODO: Nullables
        _second.NetworkSerialize(serializer);
    }
}
