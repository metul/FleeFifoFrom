using System.Collections.Generic;
using System.Linq;

public class RiotStepCommand : ActionCommand
{

    protected DKnight _knight;
    protected DPosition _to;
    protected DPosition _from;
    protected DMeeple[] _onTheWay;
    public RiotStepCommand(
        ulong issuerID,
        DPlayer.ID playerId,
        DKnight knight,
        DPosition to
    ) : base(issuerID, playerId, null)
    {
        _knight = knight;
        _to = to;
        _from = _knight.Position.Current;
        _onTheWay = GameState.Instance.AllAtPosition(_to);
    }

    public override void Execute()
    {
        base.Execute();
        _knight.Position.Current = _to;
        foreach (var meeple in _onTheWay)
        {
            if (meeple.GetType().IsSubclassOf(typeof(DVillager)))
            {
                ((DVillager) meeple).Injure();
                GameState.Instance.PlayerById(_playerId)?.Honor.Lose(1);
            }
        }

        if (_to.IsFinal)
        {
            // Q: Do you get honor for rescuing someone else's knight?
            var honor = _knight.Authorize(_playerId);
            GameState.Instance.PlayerById(_playerId)?.Honor.Earn(honor);
        }
    }

    public override void Reverse()
    {
        base.Reverse();
        _knight.Position.Current = _from;
        foreach (var meeple in _onTheWay)
        {
            if (meeple.GetType().IsSubclassOf(typeof(DVillager)))
            {
                ((DVillager) meeple).Heal();
                GameState.Instance.PlayerById(_playerId)?.Honor.Earn(1);
            }
        }

        if (_to.IsFinal)
        {
            var honor = _knight.Deauthorize(_from, _playerId);
            GameState.Instance.PlayerById(_playerId)?.Honor.Lose(honor);
        }
    }

    public override bool IsFeasibile()
    {
        return GameState.Instance.AllAtPosition(
            _to,
            m => m.IsInjured() || m.GetType() == typeof(DKnight)
        ).Length == 0;
    }
}
