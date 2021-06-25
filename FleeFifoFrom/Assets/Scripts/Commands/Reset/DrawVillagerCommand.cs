using MLAPI.Serialization;

public class DrawVillagerCommand : ResetCommand, INetworkSerializable
{
    private DPosition _position;
    private DVillager _villager;

    // Default constructor needed for serialization
    public DrawVillagerCommand() : base() { }

    public DrawVillagerCommand(ulong issuerID, DVillager villager, DPosition position) : base(issuerID)
    {
        _villager = villager;
        _position = position;
        _freeCommand = true;
    }

    public override void Execute()
    {
        base.Execute();
        _villager.Draw(_position);
        GameState.Instance.UpdateVillagerBagCount();
    }

    public override void Reverse()
    {
        base.Reverse();
        _villager.UnDraw();
        GameState.Instance.UpdateVillagerBagCount();
    }

    public override bool IsFeasible()
    {
        return base.IsFeasible()
            && _villager.State == DMeeple.MeepleState.OutOfBoard
            && GameState.Instance.IsEmpty(_position)
            && _position.Row == Rules.ROWS;
            
            //&& GameState.Instance.PathExists(
                //DPosition.LastRow(),
                //_position,
                //p => GameState.Instance.IsEmpty(p)
            //);
    }

    public override void NetworkSerialize(NetworkSerializer serializer)
    {
        base.NetworkSerialize(serializer);

        if (serializer.IsReading)
            _position = new DPosition();

        _position.NetworkSerialize(serializer);

        ushort villagerID = ushort.MaxValue;
        if (!serializer.IsReading)
            villagerID = _villager.ID;

        serializer.Serialize(ref villagerID);

        if (serializer.IsReading)
            _villager = (DVillager)RegistryManager.Instance.Request(villagerID);
    }
}
