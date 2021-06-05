public class DrawVillagerCommand : ResetCommand
{
    private DPlayer.ID _playerId;
    private DVillager _villager;
    private DPosition _position;

    // Step 1: Pick random villager from remaining pool
    // VillagerBag.DrawVillager() outside this command
    // TODO: Draw Villager: show villager preview before init
    public DrawVillagerCommand(ulong issuerID, DVillager villager, DPosition position) : base(issuerID)
    {
        _villager = villager;
        _position = position;
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

    public override bool IsFeasibile()
    {
        return GameState.Instance.InBag(_villager);
    }
}
