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
            var tile = _fieldManager.VacantBattlefieldTile(((DKnight) Core).Owner);
            if (tile != null)
                SetTo(tile, instantly);
            else
                base.SetTo((DPosition) null, instantly);
        }
        else
        {
            base.SetTo(position, instantly);
        }
    }
}
