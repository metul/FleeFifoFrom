public class DWorker : DObject
{
  public enum WorkerState
  {
    InPool, // --> worker is in some player's worker pool
    OnMat,  // --> worker is on some action mat
  }

  public DPlayer.ID Owner { get; private set; }
  public DPlayer.ID ControlledBy { get; private set; }
  public WorkerState State { get; private set; } = WorkerState.InPool;
  public bool Available { get { return State == WorkerState.InPool; } }

  public DWorker(DPlayer.ID owner): base()
  {
    Owner = owner;
    ControlledBy = owner;
  }

  public void Poach(DPlayer.ID poacher)
  {
    ControlledBy = poacher;
  }

  public void UnPoach()
  {
    ControlledBy = Owner;
  }

  public void Consume()
  {
    State = WorkerState.OnMat;
  }

  public void UnConsume()
  {
    State = WorkerState.InPool;
  }

  public void Release()
  {
    State = WorkerState.OnMat;
    ControlledBy = Owner;
  }
}
