public class DVillager : DMeeple
{
  public enum HealthStates
  {
    Injrued,   // --> the villager is injured
    Healthy,   // --> the villager is not injured
  }

  public Observable<HealthStates> Health = new Observable<HealthStates>(HealthStates.Healthy);
  public DPlayer.ID? Rescuer { get; protected set; }

  public void Injure()
  {
    Health.Current = HealthStates.Injrued;
    QueueState.Current = DMeeple.MeepleQueueState.UnTapped;
  }

  public void Heal()
  {
    Health.Current = HealthStates.Healthy;
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
      Position.Current = position;
    }
  }

  public void UnDraw()
  {
    if (State == MeepleState.InQueue)
    {
      State = MeepleState.OutOfBoard;
      Position.Current = null;
    }
  }
}

// TODO: maybe later they need to go to their own files?
public class DCommoner : DVillager {}
public class DElder : DVillager {}
public class DChild : DVillager {}