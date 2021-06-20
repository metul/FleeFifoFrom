public class DKnight : DMeeple
{
    public DPlayer.ID Owner { get => _owner; private set => _owner = value; }
    private DPlayer.ID _owner;

    public DKnight(DPlayer.ID owner, DPosition position, MeepleState state) : base(position, state)
    {
        Owner = owner;
    }
    public DKnight(DPlayer.ID owner, DPosition position) : this(owner, position, MeepleState.InQueue) { }
    public DKnight(DPlayer.ID owner) : this(owner, null, MeepleState.OutOfBoard) { }

    public void Retreat(DPosition position)
    {
        if (State == MeepleState.OutOfBoard && position.IsValid)
        {
            Position.Current = position;
            State = MeepleState.InQueue;
        }
    }

    public void UnRetreat()
    {
        if (State == MeepleState.InQueue)
        {
            State = MeepleState.OutOfBoard;
            Position.Current = null;
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
        GameState.Instance.PlayerById(player).OnDeAuthorize?.Invoke();
        return (ushort)(player == Owner ? 0 : 1);
    }

    /// <summary>
    /// <returns>
    ///   honor returned (if rescuing opponents knight)
    /// </returns>
    /// </summary>
    public ushort Deauthorize(DPosition previousPosition, DPlayer.ID player)
    {
        _deauthorize(previousPosition);
        GameState.Instance.PlayerById(player).OnDeAuthorize?.Invoke();
        return (ushort)(player == Owner ? 0 : 1);
    }
}
