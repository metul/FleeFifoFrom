public class DWorker : DObject
{
    public enum WorkerState
    {
        InPool, // --> worker is in some player's worker pool
        OnMat,  // --> worker is on some action mat
    }
    public DPlayer.ID Owner { get => _owner; private set => _owner = value; }
    private DPlayer.ID _owner;
    public DPlayer.ID ControlledBy { get => _controlledBy; private set => _controlledBy = value; }
    private DPlayer.ID _controlledBy; 
    public WorkerState State { get => Position.Current.Player != null ? WorkerState.InPool : WorkerState.OnMat; }

    public Observable<DActionPosition> Position;

    public bool Available { get { return State == WorkerState.InPool; } }

    public DWorker(DPlayer.ID owner) : base()
    {
        Owner = owner;
        ControlledBy = owner;

        // init in player pool
        Position = new Observable<DActionPosition>(new DActionPosition(owner));
    }

    public void Consume(DActionPosition.TileId tileId)
    {
        Position.Current = new DActionPosition(tileId);
    }

    public void UnConsume(DPlayer.ID playerId)
    {
        ControlledBy = playerId;
        Position.Current = new DActionPosition(ControlledBy);
    }

    public void Release()
    {
        UnConsume(Owner);
    }

    public void Poach(DPlayer.ID poacher)
    {
        UnConsume(poacher);
    }

    // Return == UnPoach == UnRelease 
    public void Return(DPlayer.ID previousOwner, DActionPosition.TileId tileId)
    {
        ControlledBy = previousOwner;
        Consume(tileId);
    }
}
