using System;
using UnityEngine;

public class DVillager : DMeeple
{
  public enum HealthStates
  {
    Injured,   // --> the villager is injured
    Healthy,   // --> the villager is not injured
  }

  public Observable<HealthStates> Health = new Observable<HealthStates>(HealthStates.Healthy);
  public DPlayer.ID? Rescuer { get; protected set; }

  public Action<DVillager> OnInitVisual;
  public Action<DVillager> OnDestroyVisual;

  public void Injure()
  {
    Health.Current = HealthStates.Injured;
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

  public void Deauthorize(DPosition previousPosition)
  {
    _deauthorize(previousPosition);
    Rescuer = null;
  }

  public void Draw(DPosition position)
  {
    if (State == MeepleState.OutOfBoard && position.IsValid)
    {
      State = MeepleState.InQueue;
      Position.Current = position;
      OnInitVisual.Invoke(this);
    }
  }

  public void UnDraw()
  {
    if (State == MeepleState.InQueue)
    {
      State = MeepleState.OutOfBoard;
      Position.Current = null;
      OnDestroyVisual.Invoke(this);
    }
  }
}

// TODO: maybe later they need to go to their own files?
public class DCommoner : DVillager {}
public class DElder : DVillager {}
public class DChild : DVillager {}