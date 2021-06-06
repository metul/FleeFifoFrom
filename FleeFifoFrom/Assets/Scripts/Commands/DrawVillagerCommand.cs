public class DrawVillagerCommand : Command
{
    private DPosition _position;
    private DVillager _villager;
    
    public DrawVillagerCommand(ulong issuerID, DVillager villager, DPosition position) : base(issuerID)
    {
        _villager = villager;
        _position = position;
    }

    public override void Execute()
    {
        _villager.Draw(_position);
    }

    public override void Reverse()
    {
        _villager.UnDraw();
    }

    public override bool IsFeasibile()
    {
        return _villager.State == DMeeple.MeepleState.OutOfBoard;
    }
}
