public class DKnight : DMeeple
{
  public DPlayer.ID Owner { get; private set; }

  public DKnight(DPlayer.ID owner, DPosition position, MeepleState state): base(position, state)
  {
    Owner = owner;
  }

  public DKnight(DPlayer.ID owner, DPosition position): this(owner, position, MeepleState.InQueue) {}
  public DKnight(DPlayer.ID owner): this(owner, null, MeepleState.OutOfBoard){}

  public void Retreat(DPosition position)
  {
    if (State == MeepleState.OutOfBoard && position.IsValid)
    {
      Position.Current = position;
      State = MeepleState.InQueue;
    }
  }

  /// <summary>
  /// <returns>
  ///   honor gained (if rescuing opponents knight)
  /// </returns>
  /// </summary>
  public ushort Authorize(DPlayer.ID player)
  {
    _authorize();
    return (ushort) (player == Owner ? 0 : 1);
  }

  /// <summary>
  /// <returns>
  ///   honor returned (if rescuing opponents knight)
  /// </returns>
  /// </summary>
  public ushort Deauthorize(DPosition previousPosition, DPlayer.ID player)
  {
    _deauthorize(previousPosition);
    return (ushort) (player == Owner ? 0 : 1);
  }
}
