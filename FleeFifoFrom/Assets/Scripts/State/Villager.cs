public class DVillager : DMeeple
{
    public enum HealthStates
    {
        Injured,   // --> the villager is injured
        Healthy,   // --> the villager is not injured
    }

    public Observable<HealthStates> Health = new Observable<HealthStates>(HealthStates.Healthy);
    public DPlayer.ID? Rescuer { get => _rescuer; protected set => _rescuer = value; }
    private DPlayer.ID? _rescuer;

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
        GameState.Instance.PlayerById(rescuer).OnDeAuthorize?.Invoke();
    }

    public void Deauthorize(DPosition previousPosition)
    {
        _deauthorize(previousPosition);
        if (Rescuer != null)
            GameState.Instance.PlayerById((DPlayer.ID)Rescuer).OnDeAuthorize?.Invoke();
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
public class DFarmer : DVillager { }

public class DScholar : DVillager { }

public class DMerchant : DVillager { }