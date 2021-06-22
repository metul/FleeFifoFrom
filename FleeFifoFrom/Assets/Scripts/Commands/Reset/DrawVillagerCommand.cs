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
            && _position.GetRow() == Rules.ROWS;
            
            //&& GameState.Instance.PathExists(
                //DPosition.LastRow(),
                //_position,
                //p => GameState.Instance.IsEmpty(p)
            //);
    }
}
