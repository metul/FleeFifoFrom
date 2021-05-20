using System.Collections.Generic;

public class GameStateDummy
{
    static GameStateDummy() {}
    private GameStateDummy() {}
    public Player CurrentPlayer { get; private set; }

    public List<Player> PlayerList = new List<Player>();

    public static GameStateDummy Instance { get; } = new GameStateDummy();
}
