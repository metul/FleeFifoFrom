public class ReprioritizeCommand : ResetCommand
{
    private DPrio _prio;
    private bool _inscrease;

    public ReprioritizeCommand(ulong issuerID, DPrio prio, bool increase) : base(issuerID)
    {
        _prio = prio;
        _inscrease = increase;
    }

    public override void Execute()
    {
        base.Execute();
        if(_inscrease)
            _prio.Increase();
        else
            _prio.Decrease();
    }

    public override void Reverse()
    {
        base.Reverse();
        if(_inscrease)
            _prio.Decrease();
        else
            _prio.Increase();
    }

    public override bool IsFeasible()
    {
        return _inscrease ? _prio.IsIncreasable : _prio.IsDecreasable;
    }
}
