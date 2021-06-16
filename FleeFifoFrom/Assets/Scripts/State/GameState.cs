using System;
using System.CodeDom;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameState
{
  #region singleton instance management

  private static GameState _instance = null;
  public static GameState Instance
  {
    get
    {
      if (_instance == null) {
        Initialize(DPlayer.CreateAnonymousPlayers());
      }
      return _instance;
    }
  }

  public static void Initialize(DPlayer[] players)
  {
    if (_instance == null) {
      _instance = new GameState(players);
    }
  }

  #endregion

  #region properties
  public enum TurnTypes
  {
    ActionTurn,
    ResetTurn,
  }

  public DPosition[][] Board { get; private set; }
  public DWorker[] Workers { get; private set; }
  public DKnight[] Knights { get; private set; }
  public DMeeple[] Meeple { get; private set; }
  public DVillager[] Villagers { get; private set; }
  public readonly DPlayer[] Players;
  public Dictionary<Type, DPrio> Priorities { get; private set; }
  
  public ushort TurnPlayerIndex;
  public TurnTypes TurnType;

  // notification for the visual managers that some changes happened
  public Action<TurnTypes> OnTurnChange;
  public Action OnUndo;
  
  public Observable<int> TurnActionCount;
  public Observable<int> KnightsFightingCount;
  public bool TurnActionPossible => TurnActionCount.Current < Rules.TURN_ACTION_LIMIT;

  #endregion

  #region initialization

  public GameState(DPlayer[] players)
  {
    Players = players;
    _initializeBoard();
    _initializeWorkers();


    _initializeKnights();
    _initializeVillagers();
    _initPrio();

    Meeple = new DMeeple[Villagers.Length + Knights.Length];
    Villagers.CopyTo(Meeple, 0);
    Knights.CopyTo(Meeple, Villagers.Length);

    TurnActionCount = new Observable<int>(0);
    KnightsFightingCount = new Observable<int>((Rules.KNIGHT_COUNT - 1) * Players.Length);

    DrawMeeple(); // TODO: Comment out for networking
    _initializeTurn();
  }

  private void _initializeBoard()
  {
    Board = new DPosition[Rules.ROWS][];

    for (int i = 0; i < Rules.ROWS; i++) {
      Board[i] = new DPosition[i + 1];
      for (int j = 0; j < i + 1; j++) {
        Board[i][j] = new DPosition((ushort) (i + 1), (ushort) (j + 1));
      }
    }
  }

  private void _initializeWorkers()
  {
    Workers = new DWorker[Rules.WORKER_COUNT * Players.Length];

    for (ushort i = 0; i < Players.Length; i++) {
      for (ushort j = 0; j < Rules.WORKER_COUNT; j++) {
        Workers[i * Rules.WORKER_COUNT + j] = new DWorker(Players[i].Id);
      }
    }
  }

  private void _initializeKnights()
  {
    Knights = new DKnight[Rules.KNIGHT_COUNT * Players.Length];

    // TODO: maybe we want to initialize with some knights in queue as well?

    for (ushort i = 0; i < Players.Length; i++) {
      for (ushort j = 0; j < Rules.KNIGHT_COUNT; j++) {
        Knights[i * Rules.KNIGHT_COUNT + j] = new DKnight(Players[i].Id);
      }
    }
  }

  private void _initializeVillagers()
  {
    List<DVillager> villagers = new List<DVillager>();

    for (ushort i = 0; i < Rules.COMMONERS_COUNT; i++)
    {
      villagers.Add(new DFarmer());
    }

    for (ushort i = 0; i < Rules.SCHOLAR_COUNT; i++)
    {
      villagers.Add(new DScholar());
    }

    for (ushort i = 0; i < Rules.MERCHANT_COUNT; i++)
    {
      villagers.Add(new DMerchant());
    }

    Villagers = villagers.ToArray();
  }

  public void DrawMeeple()
  {
    // TODO: maybe this shouldn't be here? this should be synced across the network.

    foreach (var player in Players) {
      Knights
        .First(knight => knight.Owner == player.Id)
        .Retreat(Rules.KNIGHT_POSITIONS[player.Id])
      ;
    }

    TraverseBoard(p => {
      if (IsEmpty(p))
      {
        DrawVillager().Draw(p);
      }
    });
  }

  private void _initPrio()
  {
    Priorities = new Dictionary<Type, DPrio>();
    foreach (var prioPair in Rules.START_PRIO_ARRAY)
    {
      Priorities[prioPair.Key] = new DPrio(prioPair.Value);
    }
  }

  private void _initializeTurn()
  {
    TurnPlayerIndex = 0;
    TurnType = TurnTypes.ActionTurn;
  }

  #endregion

  #region utility functions

  public void RotateTurn()
  {
    if (TurnType == TurnTypes.ActionTurn)
    {
      TurnType = TurnTypes.ResetTurn;
      TurnPlayerIndex = RangeUtils.Normalize(TurnPlayerIndex + 2, Players.Length);
    }
    else
    {
      TurnType = TurnTypes.ActionTurn;
      TurnPlayerIndex = RangeUtils.Normalize(TurnPlayerIndex - 1, Players.Length);
    }

    TurnActionCount.Current = 0;
    OnTurnChange?.Invoke(TurnType);
  }

  public DPlayer TurnPlayer()
  {
    return Players[TurnPlayerIndex];
  }

  public DPlayer? PlayerById(DPlayer.ID id)
  {
    return Players.First(p => p.Id == id);
  }

  public int PlayerScore(DPlayer.ID player)
  {
    // TODO: later objectives and castle rewards should also be added here.
    return this.AuthorizedVillagers(player).Length + PlayerById(player).Honor.Score.Current;
  }

  public void TraverseBoard(System.Action<DPosition> action)
  {
    foreach (DPosition[] row in Board)
    {
      foreach (DPosition pos in row)
      {
        action(pos);
      }
    }
  }

  public DVillager[] VillagerBag()
  {
    return Villagers.Where(v => v.State == DMeeple.MeepleState.OutOfBoard).ToArray();
  }

  public DVillager DrawVillager()
  {
    DVillager[] bag = VillagerBag();
    return bag[Random.Range(0, bag.Length)];
  }
  
  public DPrio GetPrio(DMeeple meeple)
  {
    return Priorities[meeple.GetType()];
  }

  public ushort GetRowMaxPriority(ushort row)
  {
    var rowArray = Board[row-1];
    ushort maxPrio = (ushort) DPrio.PrioValue.Low;

    foreach (DPosition pos in rowArray)
    {
      var currentMeeple = AtPosition(pos);
      if (currentMeeple == null) continue;
      
      // update max prio if necessary
      var currentPrio = (ushort) GetPrio(currentMeeple).Value.Current;
      if (currentPrio >  maxPrio)
        maxPrio = currentPrio;
    }
    return maxPrio;
  }

  public bool CheckPriority(DMeeple meeple)
  {
    var prio = GetPrio(meeple);
    var rowPrio = GetRowMaxPriority(meeple.Position.Current.Row);
    return (ushort) prio.Value.Current >= rowPrio;
  }

  /// <summary>
  /// Returns the meeple (nullable) at given position on the board.
  /// </summary>
  public DMeeple? AtPosition(DPosition position)
  {
    return Meeple.FirstOrDefault(m => (
        m.State == DMeeple.MeepleState.InQueue
        && m.Position.Current != null && m.Position.Current.Equals(position)
      ));
  }

  /// <summary>
  /// Returns whether or not the given position on the board is empty.
  /// </summary>
  public bool IsEmpty(DPosition position)
  {
    return AtPosition(position) == null;
  }
  #endregion
}
