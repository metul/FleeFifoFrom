public class MoveVillagerCommand : ResetCommand
{
  private DMeeple _meeple;
  private DPosition _to;
  private DPosition _from;

  public MoveVillagerCommand(ulong issuerID, DMeeple meeple, DPosition to) : base(issuerID)
  {
    _meeple = meeple;
    _to = to;
    _from = meeple.Position.Current;
    _freeCommand = true;
  }

  public override void Execute()
  {
    base.Execute();
    _meeple.Position.Current = _to;
  }

  public override void Reverse()
  {
    base.Reverse();
    _meeple.Position.Current = _from;
  }

  public override bool IsFeasible()
  {
    return base.IsFeasible()
      && GameState.Instance.IsEmpty(_to)
      && _meeple.Position.Current.CanMoveTo(_to);
  }
}