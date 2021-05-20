using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A bunch of utility functions for GameState
/// </summary>
public static class GameStateUtils {

  /// <summary>
  /// Will return all the meeple on the board that match given condition.
  /// </summary>
  public static DMeeple[] MatchingMeepleOnBoard(this GameState state, System.Func<DMeeple, bool> check) {
    List<DMeeple> result = new List<DMeeple>();

    state.TraverseBoard(p => {
        DMeeple? meeple = state.AtPosition(p);
        if (meeple != null && check(meeple)) {
          result.Add(meeple);
        }
    });

    return result.ToArray();
  }

  /// <summary>
  /// Will return all the injured meeple on the board.
  /// </summary>
  public static DMeeple[] InjuredMeeple(this GameState state) {
    return state.MatchingMeepleOnBoard(m => (
      m.GetType() == typeof(DVillager) &&
      ((DVillager) m).HealthState == DVillager.VillagerHealthState.Injrued
    ));
  }

  /// <summary>
  /// Will return all the positions on the board that match given condition.
  /// </summary>
   public static DPosition[] MatchingPositions(this GameState state, System.Func<DPosition, bool> check) {
    List<DPosition> result = new List<DPosition>();

    state.TraverseBoard(p => {
      if (check(p)) {
        result.Add(p);
      }
    });

    return result.ToArray();
  }

  /// <summary>
  /// Will return all the empty positions on the board.
  /// </summary>
  public static DPosition[] EmptyPositions(this GameState state) {
    return state.MatchingPositions(p => state.IsEmpty(p));
  }

  /// <summary>
  /// Will return all the villagers who were authorized by given player.
  /// </summary>
  public static DVillager[] AuthorizedVillagers(this GameState state, PlayerID player) {
    return state.Villagers.Where(v => v.State == DMeeple.MeepleState.Authorized && v.Rescuer == player).ToArray();
  }

  /// <summary>
  /// Will return all workers available to a player (including those whom they have poached).
  /// </summary>
  public static DWorker[] AvailableWorkers(this GameState state, PlayerID player) {
    return state.Workers.Where(w => w.State == DWorker.WorkerState.InPool && w.ControlledBy == player).ToArray();
  }

  /// <summary>
  /// Returns the meeple (nullable) at given position on the board.
  /// </summary>
  public static DMeeple? AtPosition(this GameState state, DPosition position) {
    return state.Meeple.First(m => (
      m.State == DMeeple.MeepleState.InQueue
      && m.Position != null && m.Position.Equals(position)
    ));
  }

  /// <summary>
  /// Returns whether or not the given position on the board is empty.
  /// </summary>
  public static bool IsEmpty(this GameState state, DPosition position) {
    return state.AtPosition(position) == null;
  }
}
