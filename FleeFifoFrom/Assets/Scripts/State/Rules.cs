public class Rules {
  public static readonly PlayerID[] DEFAULT_PLAYERS = {
    PlayerID.Red,
    PlayerID.Blue,
    PlayerID.Green,
    PlayerID.Yellow,
  };

  public const ushort WORKER_COUNT = 4;

  public const ushort KNIGHT_COUNT = 4;

  public const ushort ROWS = 5;

  // TODO: this should become a break down based on villager type instead of one value.
  public const ushort VILLAGERS_COUNT = 32; 
}
