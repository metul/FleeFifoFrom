public class AuthorizeCommand : ActionCommand
{
    private readonly DMeeple _meeple;
    private readonly DPosition _position;

    public AuthorizeCommand(ulong issuerID, DPlayer player, DWorker worker, DMeeple meeple) : base(issuerID, player, worker)
    {
        _actionId = DActionPosition.TileId.Authorize;
        _meeple = meeple;
        _position = meeple.Position.Current;
    }

    public override void Execute()
    {   
        base.Execute();

        // Priority Check before authorizing. 
        if (!GameState.Instance.CheckPriority(_meeple))
            _player.Honor.Lose();

        if (_meeple.GetType().IsSubclassOf(typeof(DVillager)))
        {
            ((DVillager) _meeple).Authorize(_player.Id);
        }
        else if (_meeple.GetType() == typeof(DKnight))
        {
            var honor = ((DKnight) _meeple).Authorize(_player.Id);
            GameState.Instance.PlayerById(_worker.ControlledBy)?.Honor.Earn(honor);
        }
        
        _player.OnDeAuthorize?.Invoke();
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
            var honor = ((DKnight) _meeple).Deauthorize(_position, _player.Id);
            GameState.Instance.PlayerById(_worker.ControlledBy)?.Honor.Lose(honor);
        }

        // Priority Check after deauthorizing. 
        if (!GameState.Instance.CheckPriority(_meeple))
            _player?.Honor.Earn();
    }

    public override bool IsFeasible()
    {
        var endpoint = new DPosition(1, 1);

        return base.IsFeasible() && _meeple != null && _meeple.IsHealthy() && (
            _position.Equals(endpoint) || (
                GameState.Instance.IsEmpty(endpoint) &&
                GameState.Instance.PathExists(
                    _position,
                    endpoint,
                    _p => _p.Equals(_position) || GameState.Instance.IsEmpty(_p)
                )
            )
        );
    }
}
