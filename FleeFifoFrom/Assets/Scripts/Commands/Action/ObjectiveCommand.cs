using MLAPI.Serialization;

public class ObjectiveCommand : ActionCommand, INetworkSerializable
{
    // Default constructor needed for serialization
    public ObjectiveCommand() : base() { }

    public ObjectiveCommand(ulong issuerID, DPlayer player, DWorker worker) : base(issuerID, player, worker)
    {
    }

    public override void Execute()
    {
        base.Execute();
        //CurrentPlayer.add(Card);
    }

    public override void Reverse()
    {
        base.Reverse();
        //CurrentPlayer.remove(Card)
    }

    public override bool IsFeasible()
    {
        return base.IsFeasible();
    }

    public override void NetworkSerialize(NetworkSerializer serializer)
    {
        base.NetworkSerialize(serializer);
    }
}
