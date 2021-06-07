public class AuthorizeCommand : ActionCommand
{
    private readonly DMeeple _meeple;
    private readonly DPosition _position;

    public AuthorizeCommand(ulong issuerID, DPlayer.ID playerId, DWorker worker, DMeeple meeple) : base(issuerID, playerId, worker)
    {
        _actionId = DActionPosition.TileId.Authorize;
        _meeple = meeple;
        _position = meeple.Position.Current;
    }

    public override void Execute()
    {   
        base.Execute();

        if (_meeple.GetType().IsSubclassOf(typeof(DVillager)))
        {
            ((DVillager) _meeple).Authorize(_playerId);
        }
        else if (_meeple.GetType() == typeof(DKnight))
        {
            var honor = ((DKnight) _meeple).Authorize(_playerId);
            GameState.Instance.PlayerById(_worker.ControlledBy)?.Honor.Earn(honor);
        }
        
        GameState.Instance.PlayerById(_playerId).OnDeAuthorize?.Invoke();
    }

    public override void Reverse()
    {
        base.Reverse();
        
        if (_meeple.GetType().IsSubclassOf(typeof(DVillager)))
        {
            ((DVillager) _meeple).Deauthorize(_position);
        }
        else if (_meeple.GetType() == typeof(DKnight))
        {
            var honor = ((DKnight) _meeple).Deauthorize(_position, _playerId);
            GameState.Instance.PlayerById(_worker.ControlledBy)?.Honor.Lose(honor);
        }
    }

    public override bool IsFeasible()
    {
        return base.IsFeasible();
    }
}
