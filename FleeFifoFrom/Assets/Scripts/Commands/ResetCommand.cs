using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCommand : Command
{
    public ResetCommand(ulong issuerID) : base(issuerID) { }

    public override void Execute()
    {
        // TODO: Do reset specific stuff?
    }

    public override void Reverse()
    {
        // TODO: Undo reset specific stuff?
    }
}
