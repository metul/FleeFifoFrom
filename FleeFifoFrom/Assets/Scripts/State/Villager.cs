public class DVillager : DMeeple
{
  public enum VillagerHealthState
  {
    Injrued,   // --> the villager is injured
    Healthy,   // --> the villager is not injured
  }

  public VillagerHealthState HealthState { get; protected set; } = VillagerHealthState.Healthy;
  public DPlayer.ID? Rescuer { get; protected set; }

  public void Injure()
  {
    HealthState = VillagerHealthState.Injrued;
    QueueState = DMeeple.MeepleQueueState.UnTapped;
  }

  public void Heal()
  {
    HealthState = VillagerHealthState.Healthy;
  }

  public void Authorize(DPlayer.ID rescuer)
  {
    _authorize();
    Rescuer = rescuer;
  }

  public void Deauthorize()
  {
    _deauthorize();
    Rescuer = null;
  }

  public void Draw(DPosition position)
  {
    if (State == MeepleState.OutOfBoard && position.IsValid)
    {
      State = MeepleState.InQueue;
      Position = position;
    }
  }

  public void UnDraw()
  {
    if (State == MeepleState.InQueue)
    {
      State = MeepleState.OutOfBoard;
      Position = null;
    }
  }
}

public class DCommoner : DVillager {}
public class DElder : DVillager {}
public class DChild : DVillager {}