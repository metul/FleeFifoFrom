public class DMeeple : DObject
{
  public enum MeepleState
  {
    OutOfBoard, // --> the meeple is for some reason not on the board yet
    InQueue,    // --> the meeple is in the queue
    Authorized, // --> the meeple is authorized to enter the castle
  }

  public enum MeepleQueueState
  {
    Tapped,     // --> the meeple is tapped
    UnTapped,   // --> the meeple is untapped
  }

  public MeepleState State { get; protected set; } = MeepleState.OutOfBoard;
  public MeepleQueueState QueueState { get; protected set; } = MeepleQueueState.UnTapped;
  public DPosition? Position { get; protected set; }

  public DMeeple(DPosition position, MeepleState state): base()
  {
    Position = position;
    State = state;
  }

  public DMeeple(DPosition position): this(position, MeepleState.InQueue) {}
  public DMeeple(): this(null, MeepleState.OutOfBoard) {}

  // --> authorize is protected because it behaves a bit differently
  // --> based on the meeple being authorized.
  protected void _authorize()
  {
    State = MeepleState.Authorized;
    Position = null;
  }

  protected void _deauthorize()
  {
    State = MeepleState.InQueue;
  }

  public void Tap()
  {
    QueueState = MeepleQueueState.Tapped;
  }

  public void UnTap()
  {
    QueueState = MeepleQueueState.UnTapped;
  }

  public void Move(DPosition position)
  {
    if (State == MeepleState.InQueue && Position.CanMoveTo(position))
    {
      Position = position;
    }
  }

  public void MoveBack(DPosition position)
  {
    if (State == MeepleState.InQueue && position.CanMoveTo(Position))
    {
      Position = position;
    }
  }

  public void Teleport(DPosition position)
  {
    if (State == MeepleState.InQueue && position.IsValid)
    {
      Position = position;
    }
  }
}
