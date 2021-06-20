using MLAPI;
using MLAPI.Logging;
using MLAPI.Serialization;
using UnityEngine;

public class Countermand : ResetCommand, INetworkSerializable
{
    // Default constructor needed for serialization
    public Countermand() : base() { }

    public Countermand(ulong issuerID) : base(issuerID)
    {
    }

    public override void Execute()
    {
        base.Execute();

        //Player.add(Card);

        /*
        if (CurrentPlayer.count(Card) >= 2)
        {
            Player.remove(Card);
        }
        */
        NetworkLog.LogInfoServer($"Countermand executed ({_issuerID})");
    }

    public override void Reverse()
    {
        base.Reverse();
        NetworkLog.LogInfoServer($"Countermand reversed ({_issuerID})");
    }

    public override bool IsFeasible()
    {
        return base.IsFeasible();
        //TODO. Always possible, as long as pawn available
    }

    public override void NetworkSerialize(NetworkSerializer serializer)
    {
        base.NetworkSerialize(serializer);
    }
}
