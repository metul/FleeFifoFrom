using System.Collections.Generic;

public class Rules
{
  public static readonly DPlayer.ID[] DEFAULT_PLAYERS = {
    DPlayer.ID.Red,
    DPlayer.ID.Blue,
    DPlayer.ID.Green,
    DPlayer.ID.Yellow,
  };

  public static readonly Dictionary<DPlayer.ID, DPosition> KNIGHT_POSITIONS = new Dictionary<DPlayer.ID, DPosition>() {
    { DPlayer.ID.Red, new DPosition(3, 1) },
    { DPlayer.ID.Blue, new DPosition(3, 3) },
    { DPlayer.ID.Green, new DPosition(4, 2) },
    { DPlayer.ID.Yellow, new DPosition(4, 3) }
  };

  public static readonly short[] HONOR_VALUES = { -15, -12, -9, -6, -3, -1, 0, 1, 3, 6, 9, 12, 15 };
  
  public const ushort WORKER_COUNT = 4;

  public const ushort KNIGHT_COUNT = 5;

  public const ushort ROWS = 5;

  public const ushort COMMONERS_COUNT = 32; 

  public const ushort ELDERS_COUNT = 12;

  public const ushort CHILDREN_COUNT = 12;
}
