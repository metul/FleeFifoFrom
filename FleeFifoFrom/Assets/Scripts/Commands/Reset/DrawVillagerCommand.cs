public class DrawVillagerCommand : ResetCommand
{
    private DPosition _position;
    private DVillager _villager;
    
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
    }

    public override void Reverse()
    {
        base.Reverse();
        _villager.UnDraw();
    }

    public override bool IsFeasible()
    {
        return base.IsFeasible()
            && _villager.State == DMeeple.MeepleState.OutOfBoard
            && GameState.Instance.IsEmpty(_position)
            && GameState.Instance.PathExists(
                DPosition.LastRow(),
                _position,
                p => GameState.Instance.IsEmpty(p)
            );
    }
}
