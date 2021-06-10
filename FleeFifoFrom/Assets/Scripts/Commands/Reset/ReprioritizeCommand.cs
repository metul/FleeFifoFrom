public class ReprioritizeCommand : ResetCommand
{
    private Tile _tile;

    public ReprioritizeCommand(ulong issuerID, Tile tile) : base(issuerID)
    {
        _tile = tile;
    }

    public override void Execute()
    {   
        base.Execute();
        // Meeple.Priority.ChangePriority(old,new);
        // old, new include: HIGH, MED, LOW
        //Can move from any current position to any new position

    }

    public override void Reverse()
    {
        base.Reverse();
        // Meeple.Priority.ChangePriority(new,old);
    }

    public override bool IsFeasible()
  {
    return base.IsFeasible();
        //TODO: Reminder to check if any unique feasibility
        //TODO: I think this action is always feasible after base
  }
}
