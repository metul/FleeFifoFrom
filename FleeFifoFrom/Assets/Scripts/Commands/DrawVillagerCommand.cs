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
        // TODO: Move random villager generation into the command?
        // TODO: Ideally need to view villager and then choose tile. 
        // TODO: Can do the reverse order for now, i.e. tile then vill
        _tile.SetMeeple(_villager);
    }

    public override void Reverse()
    {
        base.Reverse();
        _tile.RemoveMeeple();
        Destroy(_villager.gameObject);
    }
}
