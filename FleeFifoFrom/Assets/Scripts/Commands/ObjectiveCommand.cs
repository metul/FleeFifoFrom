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
        if (CurrentPlayer.count(Card) >= 5)
        { this.ActionPossible = false; }
    }

}
