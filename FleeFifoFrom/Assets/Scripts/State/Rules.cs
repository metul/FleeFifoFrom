using System;
using System.Collections.Generic;

public class Rules
{
  public static readonly DPlayer.ID[] DEFAULT_PLAYERS = {
    DPlayer.ID.Red,
    DPlayer.ID.Blue,
    DPlayer.ID.Yellow,
    DPlayer.ID.Green,
  };

  public static readonly Dictionary<DPlayer.ID, DPosition> KNIGHT_POSITIONS = new Dictionary<DPlayer.ID, DPosition>() {
    { DPlayer.ID.Red, new DPosition(3, 1) },
    { DPlayer.ID.Blue, new DPosition(3, 3) },
    { DPlayer.ID.Yellow, new DPosition(4, 2) },
    { DPlayer.ID.Green, new DPosition(4, 3) }
  };

  public const int MAX_PLAYER_COUNT = 4;

  public static readonly short[] HONOR_VALUES = { -15, -12, -9, -6, -3, -1, 0, 1, 3, 6, 9, 12, 15 };
  
  public const ushort WORKER_COUNT = 2;

  public const ushort KNIGHT_COUNT = 4;

  public const ushort ROWS = 5;

  public const ushort COMMONERS_COUNT = 10; 

  public const ushort SCHOLAR_COUNT = 10;

  public const ushort MERCHANT_COUNT = 10;

  public const ushort TURN_ACTION_LIMIT = 2;

  public const ushort GIANT_STRENGTH = 8;
  
  public static readonly Dictionary<Type, DPrio.PrioValue> START_PRIO_ARRAY = new Dictionary<Type, DPrio.PrioValue>()
  {
    {typeof(DFarmer), DPrio.PrioValue.Low},
    {typeof(DMerchant), DPrio.PrioValue.Medium},
    {typeof(DScholar), DPrio.PrioValue.Medium},
    {typeof(DKnight), DPrio.PrioValue.High}
  };
}
