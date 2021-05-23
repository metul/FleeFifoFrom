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

    public override bool IsFeasibile()
    {
        // TODO: this is dummy?
        if (GameStateDummy.Instance.CurrentPlayer.CardCount >= 5)
        {
            return true;
        }

        return false;
    }

    public ObjectiveCommand(ulong issuerID) : base(issuerID)
    {
    }
}
