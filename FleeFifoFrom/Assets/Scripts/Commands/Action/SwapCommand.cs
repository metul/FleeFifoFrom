using MLAPI.Serialization;

public class SwapCommand : ActionCommand, INetworkSerializable
{
    private DMeeple? _first;
    private DMeeple? _second;

    // Default constructor needed for serialization
    public SwapCommand() : base() { }

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

    public override void NetworkSerialize(NetworkSerializer serializer)
    {
        base.NetworkSerialize(serializer);

        bool isFirstNull = true;
        if (!serializer.IsReading)
            isFirstNull = (_worker == null);

        serializer.Serialize(ref isFirstNull);

        if (!isFirstNull)
        {
            ushort firstID = ushort.MaxValue;
            if (!serializer.IsReading)
                firstID = _first.ID;

            serializer.Serialize(ref firstID);

            if (serializer.IsReading)
                _first = (DMeeple)RegistryManager.Instance.Request(firstID); // TODO (metul): Do we need further type casting down the line (e.g. villager)?
        }

        bool isSecondNull = true;
        if (!serializer.IsReading)
            isSecondNull = (_worker == null);

        serializer.Serialize(ref isSecondNull);

        if (!isSecondNull)
        {
            ushort secondID = ushort.MaxValue;
            if (!serializer.IsReading)
                secondID = _second.ID;

            serializer.Serialize(ref secondID);

            if (serializer.IsReading)
                _second = (DMeeple)RegistryManager.Instance.Request(secondID); // TODO (metul): Do we need further type casting down the line (e.g. villager)?
        }
    }
}
