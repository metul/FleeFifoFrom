using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Meeple
{
    public override void Initialize(DMeeple core, FieldManager fieldManager)
    {
        base.Initialize(core, fieldManager);

        if (core.GetType() == typeof(DKnight))
        {
            SetColor(ColorUtils.GetPlayerColor(((DKnight) core).Owner));
        }
    }

    protected override void SetTo(DPosition position, bool instantly = true)
    {
        if (position == null && Core.State == DMeeple.MeepleState.OutOfBoard)
        {
            var fieldManager = FindObjectOfType<FieldManager>();
            var tile = fieldManager.VacantBattlefieldTile(((DKnight) Core).Owner);
            if (tile != null)
                SetTo(tile, instantly);
            else
                base.SetTo(position, instantly);
        }
        else
        {
            base.SetTo(position, instantly);
        }
    }
}
