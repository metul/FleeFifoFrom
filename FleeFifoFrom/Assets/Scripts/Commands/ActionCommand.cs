public class ActionCommand : Command
{
    public ActionCommand(ulong issuerID) : base(issuerID) { }

    public override void Execute()
    {
        // TODO: Decrease worker count
    }

    public override void Reverse()
    {
        // TODO: Increase worker count
    }
}
