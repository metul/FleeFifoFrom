public class DVillager : DMeeple {
  public enum VillagerHealthState {
    Injrued,   // --> the villager is injured
    Healthy,   // --> the villager is not injured
  }

  public VillagerHealthState HealthState { get; protected set; } = VillagerHealthState.Healthy;
  public PlayerID? Rescuer { get; protected set; }

  public void Injure() {
    HealthState = VillagerHealthState.Injrued;
  }

  public void Heal() {
    HealthState = VillagerHealthState.Healthy;
  }

  public void Authorize(PlayerID rescuer) {
    _authorize();
    Rescuer = rescuer;
  }

  public void Deauthorize() {
    _deauthorize();
    Rescuer = null;
  }

  public void Draw(DPosition position) {
    if (State == MeepleState.OutOfBoard && position.IsValid) {
      State = MeepleState.InQueue;
      Position = position;
    }
  }

  public void UnDraw() {
    if (State == MeepleState.InQueue) {
      State = MeepleState.OutOfBoard;
      Position = null;
    }
  }
}
