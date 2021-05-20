public class DKnight : DMeeple {
  public PlayerID Owner { get; private set; }

  public DKnight(PlayerID owner, DPosition position, MeepleState state): base(position, state) {
    Owner = owner;
  }

  public DKnight(PlayerID owner, DPosition position): this(owner, position, MeepleState.InQueue) {}
  public DKnight(PlayerID owner): this(owner, null, MeepleState.OutOfBoard){}

  public void Retreat(DPosition position) {
    if (State == MeepleState.OutOfBoard && position.IsValid) {
      Position = position;
      State = MeepleState.InQueue;
    }
  }

  public void Authorize() {
    _authorize();
  }

  public void Deauthorize() {
    _deauthorize();
  }
}
