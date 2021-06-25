using UnityEngine;
using MLAPI.Serialization;

public class RetreatCommand : ResetCommand, INetworkSerializable
{
    private DKnight _knight;
    private DPosition _position;

    // Default constructor needed for serialization
    public RetreatCommand() : base() { }

    public RetreatCommand(ulong issuerID, DKnight knight, DPosition position) : base(issuerID)
    {
        _knight = knight;
        _position = position;
    }

    public override void Execute()
    {
        base.Execute();
        _knight.Retreat(_position);
        GameState.Instance.KnightsFightingCount.Current--;
    }

    public override void Reverse()
    {
        base.Reverse();
        _knight.UnRetreat();
        GameState.Instance.KnightsFightingCount.Current++;
    }

    public override bool IsFeasible()
    {   
        //S.R. Can the condition be changed to only allow to last row
        // This is so that we can reuse the priority checker when stepping forward

        return base.IsFeasible()
            && GameState.Instance.IsEmpty(_position)
            && _position.Row == Rules.ROWS;
            
        /*&& GameState.Instance.PathExists(
                DPosition.LastRow(),
                _position,
                p => GameState.Instance.IsEmpty(p)
            );*/
    }

    public override void NetworkSerialize(NetworkSerializer serializer)
    {
        base.NetworkSerialize(serializer);

        ushort knightID = ushort.MaxValue;
        if (!serializer.IsReading)
            knightID = _knight.ID;

        serializer.Serialize(ref knightID);

        if (serializer.IsReading)
            _knight = (DKnight)RegistryManager.Instance.Request(knightID);

        if (serializer.IsReading)
            _position = new DPosition();

        _position.NetworkSerialize(serializer);
    }
}
