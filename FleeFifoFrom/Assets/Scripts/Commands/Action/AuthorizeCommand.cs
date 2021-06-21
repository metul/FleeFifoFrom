using MLAPI.Logging;
using MLAPI.Serialization;

public class AuthorizeCommand : ActionCommand, INetworkSerializable
{
    private DMeeple _meeple;
    private DPosition _position;

    // Default constructor needed for serialization
    public AuthorizeCommand() : base() { }

    public AuthorizeCommand(ulong issuerID, DPlayer.ID playerId, DWorker worker, DMeeple meeple) : base(issuerID, playerId, worker)
    {
        _actionId = DActionPosition.TileId.Authorize;
        _meeple = meeple;
        _position = meeple.Position.Current;
    }

    public override void Execute()
    {   
        base.Execute();

        //TODO: Priority Check before authorizing. 
        //Realistically matters for second action only

        /*if(!_meeple.Position.Current.CheckPriority())
         * {
        GameState.Instance.PlayerById(_worker.ControlledBy)?.Honor.Lose();
            }
            */

        NetworkLog.LogInfoServer($"Executed authorize command with IDs {_issuerID} / {_playerId}, " +
            $"worker ({_worker?.ID} / {_worker?.Owner} / {_worker?.ControlledBy} / {_worker?.Position.Current}) and meeple ({_meeple.ID} / {_meeple.Position.Current} / {_meeple.State})");

        if (_meeple.GetType().IsSubclassOf(typeof(DVillager)))
        {
            ((DVillager)_meeple).Authorize(_playerId);
        }
        else if (_meeple.GetType() == typeof(DKnight))
        {
            var honor = ((DKnight)_meeple).Authorize(_playerId);
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

        //TODO: Priority Check after deauthorizing. 

        /*if(!_meeple.Position.Current.CheckPriority())
         * {
        GameState.Instance.PlayerById(_worker.ControlledBy)?.Honor.Earn();
            }
            */
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

    public override void NetworkSerialize(NetworkSerializer serializer)
    {
        base.NetworkSerialize(serializer);

        ushort meepleID = ushort.MaxValue;
        if (!serializer.IsReading)
            meepleID = _meeple.ID;

        serializer.Serialize(ref meepleID);

        if (serializer.IsReading)
            _meeple = (DMeeple)ObjectManager.Instance.Request(meepleID); // TODO (metul): Do we need further type casting down the line (e.g. villager)?

        if (serializer.IsReading)
            _position = new DPosition();

        _position.NetworkSerialize(serializer);
    }
}
