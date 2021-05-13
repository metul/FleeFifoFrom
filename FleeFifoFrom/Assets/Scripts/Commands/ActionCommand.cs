public class ActionCommand : Command
{
    public ActionCommand(ulong issuerID) : base(issuerID) { }

    public void CheckWorker()
    {
        // TODO: Check that there are enough workers to use an action
        // TODO: Should this be a subsection of execute instead?
    }


    public override void Execute()
    {
        // TODO: Decrease worker count
    }

    public override void Reverse()
    {
        // TODO: Increase worker count
    }
}
