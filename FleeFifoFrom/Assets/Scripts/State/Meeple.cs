public class DMeeple : DObject
{
    // TODO: Perhaps better naming?
    public enum MeepleState
    {
        OutOfBoard, // --> the meeple is for some reason not on the board yet
        InQueue,    // --> the meeple is in the queue
                    // TODO: maybe rename this to rescued.
        Authorized, // --> the meeple is authorized to enter the castle
    }

    public enum MeepleQueueState
    {
        Tapped,     // --> the meeple is tapped
        UnTapped,   // --> the meeple is untapped
    }

    public Observable<bool> IsRioting = new Observable<bool>(false);

    // TODO: perhaps State needs to be Observable as well?
    public MeepleState State { get; protected set; } = MeepleState.OutOfBoard;
    public Observable<MeepleQueueState> QueueState = new Observable<MeepleQueueState>(MeepleQueueState.UnTapped);
    public Observable<DPosition?> Position;

    public DMeeple(DPosition position, MeepleState state) : base()
    {
        Position = new Observable<DPosition>(position);
        State = state;
    }

    public DMeeple(DPosition position) : this(position, MeepleState.InQueue) { }
    public DMeeple() : this(null, MeepleState.OutOfBoard) { }

    // --> authorize is protected because it behaves a bit differently
    // --> based on the meeple being authorized.
    protected void _authorize()
    {
        State = MeepleState.Authorized;
        Position.Current = null;
    }

    protected void _deauthorize(DPosition previousPosition)
    {
        State = MeepleState.InQueue;
        Position.Current = previousPosition;
    }

    public void Move(DPosition position)
    {

        if (State == MeepleState.InQueue && Position.Current.CanMoveTo(position))
        {
            Position.Current = position;
        }
    }

    public void MoveBack(DPosition position)
    {
        if (State == MeepleState.InQueue && position.CanMoveTo(Position.Current))
        {
            Position.Current = position;
        }
    }

    public void Teleport(DPosition position)
    {
        if (State == MeepleState.InQueue && position.IsValid)
        {
            Position.Current = position;
        }
    }
}
