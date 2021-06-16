using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RiotStepCommand : ActionCommand
{
    protected DKnight _knight;
    protected DPosition _to;
    protected DPosition _from;
    protected DMeeple[] _onTheWay;
    protected DMeeple[] _coriotors;
    protected Dictionary<ushort, DPosition> _coriotorPositions;
    public RiotStepCommand(
        ulong issuerID,
        DPlayer player,
        DKnight knight,
        DPosition to
    ) : base(issuerID, player, null)
    {
        _knight = knight;
        _to = to;
        _from = _knight.Position.Current;
        _onTheWay = GameState.Instance.AllAtPosition(_to);
        _coriotors = GameState.Instance.AllAtPositions(
            _from.Predecessors(),
            m => m.GetType().IsSubclassOf(typeof(DVillager)) && !m.IsInjured()
        );

        _coriotorPositions = new Dictionary<ushort, DPosition>();
        foreach (var coriotor in _coriotors)
        {
            _coriotorPositions[coriotor.ID] = coriotor.Position.Current;
        }
    }

    public override void Execute()
    {
        base.Execute();

        foreach (var meeple in _onTheWay)
        {
            if (meeple.GetType().IsSubclassOf(typeof(DVillager)))
            {
                ((DVillager) meeple).Injure();
                GameState.Instance.PlayerById(_player.Id)?.Honor.Lose();
            }
        }

        if (_to.IsFinal)
        {
            var honor = _knight.Authorize(_player.Id);
            GameState.Instance.PlayerById(_player.Id)?.Honor.Earn(honor);

            foreach (var coriotor in _coriotors)
            {
                ((DVillager) coriotor).Authorize(_player.Id);
                
                // done rioting
                coriotor.IsRioting.Current = false;
            }
        }
        else
        {
            _knight.Position.Current = _to;
            foreach (var coriotor in _coriotors)
            {
                coriotor.Position.Current = _from;
                coriotor.IsRioting.Current = true;
            }
        }
    }

    public override void Reverse()
    {
        base.Reverse();
        if (_to.IsFinal)
        {
            foreach (var coriotor in _coriotors)
            {
                ((DVillager) coriotor).Deauthorize(_coriotorPositions[coriotor.ID]);
                
                // start rioting again
                coriotor.IsRioting.Current = true;
            }

            var honor = _knight.Deauthorize(_from, _player.Id);
            GameState.Instance.PlayerById(_player.Id)?.Honor.Lose(honor);
        }
        else
        {
            foreach (var coriotor in _coriotors)
            {
                coriotor.Position.Current = _coriotorPositions[coriotor.ID];
                
                // TODO check if they joined recently
                coriotor.IsRioting.Current = false;
                
            }
            _knight.Position.Current = _from;
        }

        foreach (var meeple in _onTheWay)
        {
            if (meeple.GetType().IsSubclassOf(typeof(DVillager)))
            {
                ((DVillager) meeple).Heal();
                GameState.Instance.PlayerById(_player.Id)?.Honor.Earn();
            }
        }
    }

    public override bool IsFeasible()
    {
        return base.IsFeasible()
            && GameState.Instance.AllAtPosition(
                _to,
                m => m.IsInjured() || m.GetType() == typeof(DKnight)
            ).Length == 0;
    }
}
