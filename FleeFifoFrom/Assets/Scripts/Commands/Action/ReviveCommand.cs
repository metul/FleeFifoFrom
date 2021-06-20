using MLAPI.Serialization;

public class ReviveCommand : ActionCommand, INetworkSerializable
{
    //private Tile _tile;
    private readonly DMeeple _meeple;
    private readonly DPosition _position;
    // private Meeple.State _originalState; // MARK: Redundant, could be replaced with Meeple.State.Injured

    // Default constructor needed for serialization
    public ReviveCommand() : base() { }

    public ReviveCommand(ulong issuerID, DPlayer.ID playerId, DWorker worker, DMeeple meeple) : base(issuerID, playerId, worker)
    {
        _actionId = DActionPosition.TileId.Revive;
        _meeple = meeple;
        _position = meeple.Position.Current;
    }


    public override void Execute()
    {
        base.Execute();
 
        //Heal the meeple
        ((DVillager) _meeple).Heal();

        //Distribute Honor

        // TODO: who gets honor with player != token owner
        //Remove hard coded 1
        GameState.Instance.PlayerById(_worker.ControlledBy)?.Honor.Earn();
        
    }

    public override void Reverse()
    {
        base.Reverse();
        ((DVillager) _meeple).Injure();
        GameState.Instance.PlayerById(_worker.ControlledBy)?.Honor.Lose();
    }

    public override bool IsFeasible()
    {
        return base.IsFeasible() && (_meeple.IsInjured());
    }

    public override void NetworkSerialize(NetworkSerializer serializer)
    {
        base.NetworkSerialize(serializer);
        _meeple.NetworkSerialize(serializer);
        _position.NetworkSerialize(serializer);
    }
}
