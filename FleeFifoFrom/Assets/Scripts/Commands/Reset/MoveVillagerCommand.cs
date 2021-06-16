public class MoveVillagerCommand : ResetCommand
{
  private readonly DMeeple _meeple;
  private readonly DPosition _to;
  private readonly DPosition _from;
  private readonly DPlayer _player;

  public MoveVillagerCommand(ulong issuerID, DPlayer player, DMeeple meeple, DPosition to) : base(issuerID)
  {
    _player = player;
    _meeple = meeple;
    _to = to;
    _from = meeple.Position.Current;
    _freeCommand = true;
  }

  public override void Execute()
  {
    base.Execute();

        //i.e. lower honor if priority broken, then move piece as usual  
        if (!GameState.Instance.CheckPriority(_meeple))
          _player.Honor.Lose();
        
        _meeple.Position.Current = _to;
  }

  public override void Reverse()
  {
    base.Reverse();
    _meeple.Position.Current = _from;
    
    //i.e. raise honor retroactively if priority would have been broken 
    if (!GameState.Instance.CheckPriority(_meeple))
      _player.Honor.Earn();

  }

    public override bool IsFeasible()
  {
    return base.IsFeasible()
      && GameState.Instance.IsEmpty(_to)
      && _meeple.Position.Current.CanMoveTo(_to);
  }
}