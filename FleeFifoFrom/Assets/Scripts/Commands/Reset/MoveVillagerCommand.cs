using MLAPI.Serialization;

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
        //TODO if (!Meeple.CurrentRow.CheckPriority())
        //{GameState.Instance.PlayerById.Honor.Lose();}
        //i.e. lower honor if priority broken, then move piece as usual  
        _meeple.Position.Current = _to;
    }

    public override void Reverse()
    {
        base.Reverse();
        _meeple.Position.Current = _from;
        //TODO if (!Meeple.CurrentRow.CheckPriority())
        //{GameState.Instance.PlayerById.Honor.Earn();}
        //i.e. raise honor retroactively if priority would have been broken 
    }

    public override bool IsFeasible()
    {
        return base.IsFeasible()
          && GameState.Instance.IsEmpty(_to)
          && _meeple.Position.Current.CanMoveTo(_to);
    }

    public override void NetworkSerialize(NetworkSerializer serializer)
    {
        base.NetworkSerialize(serializer);
        _meeple.NetworkSerialize(serializer);
        _to.NetworkSerialize(serializer);
        _from.NetworkSerialize(serializer);
    }
}