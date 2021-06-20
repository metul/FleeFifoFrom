using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI.Serialization;

public abstract class ResetCommand : Command, INetworkSerializable
{
    protected bool _freeCommand = false;

    // Default constructor needed for serialization
    public ResetCommand() : base() { }

    // TODO include reset turn pawn system for reset commands
    public ResetCommand(ulong issuerID) : base(issuerID) { }

    public void CheckPawn()
    {
        // TODO: Check that there are enough pawns to use a reset command
        // TODO: Check that there is an option to still use the side of the reset mat (left/right)
        // TODO: Should this be a subsection of execute instead?
    }

    public override void Execute()
    {
        // TODO: Do reset specific stuff?
        // TODO: Decrement reset pawn. I think the remaining will be command specific
        if (!_freeCommand)
            GameState.Instance.TurnActionCount.Current++;
    }

    public override void Reverse()
    {
        // TODO: Undo reset specific stuff?
        // TODO: Increment reset pawn. I think the remaining will be command specific
        if (!_freeCommand)
            GameState.Instance.TurnActionCount.Current--;
    }

    public override bool IsFeasible()
    {
        return GameState.Instance.TurnType == GameState.TurnTypes.ResetTurn;
    }

    public override void NetworkSerialize(NetworkSerializer serializer)
    {
        base.NetworkSerialize(serializer);
        serializer.Serialize(ref _freeCommand);
    }
}
