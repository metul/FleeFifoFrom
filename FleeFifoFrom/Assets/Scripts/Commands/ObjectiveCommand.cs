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

    public override void CheckFeasibility()
    {
        if (GameStateDummy.Instance.CurrentPlayer.CardCount >= 5)
        { this.ActionPossible = false; }
    }

    public ObjectiveCommand(ulong issuerID) : base(issuerID)
    {
    }
}
