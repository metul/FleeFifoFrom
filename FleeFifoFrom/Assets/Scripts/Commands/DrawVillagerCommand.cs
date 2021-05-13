public class DrawVillagerCommand : ResetCommand
{
    private Tile _tile;
    private Meeple _villager;

    public DrawVillagerCommand(ulong issuerID, Meeple villager, Tile tile) : base(issuerID)
    {
        _tile = tile;
        _villager = villager;
    }

    public override void Execute()
    {
        base.Execute();
        // TODO: Move random villager generation into the command?
        _tile.SetMeeple(_villager);
    }

    public override void Reverse()
    {
        base.Reverse();
        _tile.RemoveMeeple();
        Destroy(_villager.gameObject);
    }
}
