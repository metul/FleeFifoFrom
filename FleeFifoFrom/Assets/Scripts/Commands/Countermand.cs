public class Countermand : ResetCommand
{
    public Countermand(ulong issuerID) : base(issuerID)
    {
    }

    public override void Execute()
    {
        base.Execute();

        //Player.add(Card);

        /*
        if (CurrentPlayer.count(Card) >= 2)
        {
            Player.remove(Card);
        }
        */

    }

    public override void Reverse()
    {
        base.Reverse();
    }

    public override void CheckFeasibility()
    {
        //TODO. Always possible, as long as pawn available
    }
}
