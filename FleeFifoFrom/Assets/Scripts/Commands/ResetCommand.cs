using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCommand : Command
{
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
    }

    public override void Reverse()
    {
        // TODO: Undo reset specific stuff?
        // TODO: Increment reset pawn. I think the remaining will be command specific
    }
}
