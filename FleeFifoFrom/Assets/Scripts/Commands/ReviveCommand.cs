using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveCommand : ActionCommand
{
    private Tile _tile;
    private Meeple.State _originalState; // MARK: Redundant, could be replaced with Meeple.State.Injured

    public ReviveCommand(ulong issuerID, Tile tile) : base(issuerID)
    {
        _tile = tile;
    }

    public override void Execute()
    {
        base.Execute();
        _originalState = _tile.Meeple.CurrentState;
        // TODO which state after revival?
        _tile.Meeple.CurrentState = Meeple.State.Default;
    }

    public override void Reverse()
    {
        base.Reverse();
        _tile.Meeple.CurrentState = _originalState;
    }
}
