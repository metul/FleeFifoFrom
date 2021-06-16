public class ObjectiveCommand : ActionCommand
{
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

    public ObjectiveCommand(ulong issuerID, DPlayer player, DWorker worker) : base(issuerID, player, worker)
    {
    }
}
