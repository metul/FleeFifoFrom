using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class GameState {

  #region singleton instance management

  private static GameState _instance = null;
  public static GameState Instance {
    get {
      Initialize(Rules.DEFAULT_PLAYERS);
      return _instance;
    }
  }

  public static void Initialize(PlayerID[] players) {
    if (_instance == null) {
      _instance = new GameState(players);
    }
  }

  #endregion

  #region properties
  public enum TurnTypes {
    ActionTurn,
    ResetTurn,
  }

  public DPosition[][] Board { get; private set; }
  public DWorker[] Workers { get; private set; }
  public DKnight[] Knights { get; private set; }
  public DVillager[] Villagers { get; private set; }
  readonly PlayerID[] Players;
  public ushort TurnPlayer;
  public TurnTypes TurnType;

  #endregion

  #region initialization

  public GameState(): this(Rules.DEFAULT_PLAYERS) {}

  public GameState(PlayerID[] players) {
    Players = players;
    _initializeBoard();
    _initializeWorkers();
    _initializeKnights();
    _initializeVillagers();
    _drawMeeple();
    _initializeTurn();
  }

  private void _initializeBoard() {
    Board = new DPosition[Rules.ROWS][];

    for (int i = 1; i <= Rules.ROWS; i++) {
      Board[i] = new DPosition[i];
      for (int j = 1; j <= i; j++) {
        Board[i][j] = new DPosition((ushort) i, (ushort) j);
      }
    }
  }

  private void _initializeWorkers() {
    Workers = new DWorker[Rules.WORKER_COUNT * Players.Length];

    for (ushort i = 0; i < Players.Length; i++) {
      for (ushort j = 0; j < Rules.WORKER_COUNT; j++) {
        Workers[i * Rules.WORKER_COUNT + j] = new DWorker(Players[i]);
      }
    }
  }

  private void _initializeKnights() {
    Knights = new DKnight[Rules.KNIGHT_COUNT * Players.Length];

    // TODO: maybe we want to initialize with some knights in queue as well?

    for (ushort i = 0; i < Players.Length; i++) {
      for (ushort j = 0; j < Rules.KNIGHT_COUNT; j++) {
        Knights[i * Rules.KNIGHT_COUNT + j] = new DKnight(Players[i]);
      }
    }
  }

  private void _initializeVillagers() {
    // TODO: this method should create random villager types, or based on game rules or whatever.

    Villagers = new DVillager[Rules.VILLAGERS_COUNT];

    for (ushort i = 0; i < Rules.VILLAGERS_COUNT; i++) {
      Villagers[i] = new DVillager();
    }
  }

  private void _drawMeeple() {

    // TODO: perhaps some knights also should be placed on the board?

    for (ushort i = 1; i <= Rules.ROWS; i++) {
      for (ushort j = 1; j <= i; j++) {
        DrawVillager().Draw(Board[i][j]);
      }
    }
  }

  private void _initializeTurn() {
    TurnPlayer = 0;
    TurnType = TurnTypes.ActionTurn;
  }

  #endregion

  #region utility functions

  public void rotateTurn() {
    if (TurnType == TurnTypes.ActionTurn) {
      TurnType = TurnTypes.ResetTurn;
      TurnPlayer = (ushort) ((TurnPlayer + 2) % Players.Length);
    } else {
      TurnType = TurnTypes.ActionTurn;
      TurnPlayer = (ushort) ((TurnPlayer - 1) % Players.Length);
    }
  }

  public PlayerID TurnPlayerID() {
    return Players[TurnPlayer];
  }

  public DMeeple[] MatchingMeepleOnBoard(System.Func<DMeeple, bool> check) {
    List<DMeeple> result = new List<DMeeple>();

    for (ushort i = 1; i <= Rules.ROWS; i++) {
      for (ushort j = 1; j <= i; j++) {
        DMeeple meeple = AtPosition(Board[i][j]);
        if (meeple != null && check(meeple)) {
          result.Add(meeple);
        }
      }
    }

    return result.ToArray();
  }

  public DPosition[] MatchingPositions(System.Func<DPosition, bool> check) {
    List<DPosition> result = new List<DPosition>();

    for (ushort i = 1; i <= Rules.ROWS; i++) {
      for (ushort j = 1; j <= j; j++) {
        if (check(Board[i][j])) {
          result.Add(Board[i][j]);
        }
      }
    }

    return result.ToArray();
  }

  public DMeeple? AtPosition(DPosition position) {
    return Villagers.First(v => v.Position.Equals(position));
  }

  public bool IsEmpty(DPosition position) {
    return AtPosition(position) == null;
  }

  public DPosition[] EmptyPositions() {
    return MatchingPositions(p => IsEmpty(p));
  }

  public DVillager[] VillagerBag() {
    return Villagers.Where(v => v.State == DMeeple.MeepleState.OutOfBoard).ToArray();
  }

  public DVillager DrawVillager() {
    DVillager[] bag = VillagerBag();
    return bag[Random.Range(0, bag.Length)];
  }

  public int AuthorizedVillagers(PlayerID player) {
    return Villagers.Where(v => v.State == DMeeple.MeepleState.Authorized && v.Rescuer == player).ToArray().Length;
  }

  public DWorker[] AvailableWorkers(PlayerID player) {
    return Workers.Where(w => w.State == DWorker.WorkerState.InPool && w.ControlledBy == player).ToArray();
  }

  #endregion
}
