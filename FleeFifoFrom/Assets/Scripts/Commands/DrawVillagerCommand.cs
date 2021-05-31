public class DrawVillagerCommand : ResetCommand
{
    private Tile _tile;
    private Meeple _villager;
    //TODO: Switch to villager class


    public DrawVillagerCommand(ulong issuerID, Meeple villager, Tile tile) : base(issuerID)
    {
        _tile = tile;
        _villager = villager;
    }

    public override void Execute()
    {
        base.Execute();
        // Step 1: Pick random villager from remaining pool
        // TODO: Ideally need to view villager and then choose tile. 
        // TODO: Can do the reverse order for now, i.e. tile then vill
        //TODO:
        VillagerBag.DrawVillager();
        _villager.SetTo(_tile);
    }

    public override void Reverse()
    {
        base.Reverse();
        // _tile.RemoveMeeple();
        // TODO don't call destroy from command, this should be not a mono behaviour
        // Destroy(_villager.gameObject);
    }

    public override bool IsFeasibile()
    {
        return true;
    }
}
