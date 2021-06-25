using System.Collections.Generic;

public class RiotAuthorizeCommand : ActionCommand
{
    protected DKnight _knight;
    protected DPosition _from;
    protected DMeeple[] _coriotors;
    protected Dictionary<ushort, DPosition> _coriotorPositions;

    // Default constructor needed for serialization
    public RiotAuthorizeCommand() {}
    public RiotAuthorizeCommand(
        ulong issuerID,
        DPlayer player,
        DKnight knight,
        DPosition to
    ) : base(issuerID, player, null)
    {
        _knight = knight;
        _from = _knight.Position.Current;
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

        var honor = _knight.Authorize(_player.Id);
        GameState.Instance.PlayerById(_player.Id)?.Honor.Earn(honor);

        foreach (var coriotor in _coriotors)
        {
            ((DVillager) coriotor).Authorize(_player.Id);

            // done rioting
            coriotor.IsRioting.Current = false;
        }
    }

    public override void Reverse()
    {
        base.Reverse();

        foreach (var coriotor in _coriotors)
        {
            ((DVillager) coriotor).Deauthorize(_coriotorPositions[coriotor.ID]);

            // no rioting for no one, 
            coriotor.IsRioting.Current = false;
        }

        var honor = _knight.Deauthorize(_from, _player.Id);
        GameState.Instance.PlayerById(_player.Id)?.Honor.Lose(honor);
    }

    public override bool IsFeasible()
    {
        return base.IsFeasible();
    }
}