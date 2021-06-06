using UnityEngine;

public class RetreatCommand : ResetCommand
{
    private DKnight _knight;
    private DPosition _position;

    public RetreatCommand(ulong issuerID, DKnight knight, DPosition position) : base(issuerID)
    {
        _knight = knight;
        _position = position;
    }

    public override void Execute()
    {
        base.Execute();
        _knight.Retreat(_position);
    }

    public override void Reverse()
    {
        base.Reverse();
        _knight.UnRetreat();
    }

    public override bool IsFeasible()
    {
        return base.IsFeasible()
            && GameState.Instance.IsEmpty(_position)
            && GameState.Instance.PathExists(
                DPosition.LastRow(),
                _position,
                p => GameState.Instance.IsEmpty(p)
            );
    }
}
