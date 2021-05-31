using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;

/// <summary>
/// A bunch of utility functions for GameState
/// </summary>
public static class GameStateUtils
{

  /// <summary>
  /// Will return all the meeple on the board that match given condition.
  /// </summary>
  public static DMeeple[] MatchingMeepleOnBoard(this GameState state, System.Func<DMeeple, bool> check)
  {
    List<DMeeple> result = new List<DMeeple>();

    state.TraverseBoard(p => {
        DMeeple? meeple = state.AtPosition(p);
        if (meeple != null && check(meeple))
        {
          result.Add(meeple);
        }
    });

    return result.ToArray();
  }

  /// <summary>
  /// Will return all the injured meeple on the board.
  /// </summary>
  public static DMeeple[] InjuredMeeple(this GameState state)
  {
    return state.MatchingMeepleOnBoard(m => m.IsInjured());
  }

  /// <summary>
  /// Will return all the positions on the board that match given condition.
  /// </summary>
   public static DPosition[] MatchingPositions(this GameState state, System.Func<DPosition, bool> check)
   {
    List<DPosition> result = new List<DPosition>();

    state.TraverseBoard(p => {
      if (check(p))
      {
        result.Add(p);
      }
    });

    return result.ToArray();
  }

  /// <summary>
  /// Will return all the empty positions on the board.
  /// </summary>
  public static DPosition[] EmptyPositions(this GameState state)
  {
    return state.MatchingPositions(p => state.IsEmpty(p));
  }

  // TODO: maybe move this function so that its less confusing?
  // TODO: maybe rename this to RescuedVillagers()?
  /// <summary>
  /// Will return all the villagers who were authorized by given player.
  /// </summary>
  public static DVillager[] AuthorizedVillagers(this GameState state, DPlayer.ID player)
  {
    return state.Villagers.Where(v => v.State == DMeeple.MeepleState.Authorized && v.Rescuer == player).ToArray();
  }

  /// <summary>
  /// Will return all workers available to a player (including those whom they have poached).
  /// </summary>
  public static DWorker[] AvailableWorkers(this GameState state, DPlayer.ID player)
  {
    return state.Workers.Where(w => w.State == DWorker.WorkerState.InPool && w.ControlledBy == player).ToArray();
  }

  /// <summary>
  /// Returns the meeple (nullable) at given position on the board.
  /// </summary>
  public static DMeeple? AtPosition(this GameState state, DPosition position)
  {
    try
    {
      return state.Meeple.First(m => (
        m.State == DMeeple.MeepleState.InQueue
        && m.Position.Current != null && m.Position.Current.Equals(position)
      ));
    }
    catch (InvalidOperationException e)
    {
      return null;
    }
  }
  
  public static List<DWorker> AtTilePosition(this GameState state, DActionPosition position)
  {
    var result = new List<DWorker>();
    foreach (var worker in state.Workers)
    {
      if (worker.Position.Current.Equals(position))
      {
        result.Add(worker);
      }
    }

    return result;
  } 

  // TODO: add TraversePath()?
  // TODO: add PathExists()?

  public static bool IsInjured(this DMeeple meeple)
  {
    return (
      meeple.GetType() == typeof(DVillager)
      && ((DVillager) meeple).Health.Current == DVillager.HealthStates.Injrued
    );
  }

  public static bool IsHealthy(this DMeeple meeple)
  {
    return (
      meeple.GetType() != typeof(DVillager)
      || ((DVillager) meeple).Health.Current == DVillager.HealthStates.Healthy
    );
  }

  /// <summary>
  /// Returns true if there is an injured villager at given position
  /// </summary>
  public static bool InjuredVillagerAtPosition(this GameState state, DPosition position)
  {
    var meeple = state.AtPosition(position);
    return meeple != null && meeple.IsInjured();
  }

  /// <summary>
  /// Returns true if there is a healthy meeple at given position
  /// </summary>
  public static bool HealthyMeepleAtPosition(this GameState state, DPosition position)
  {
    var meeple = state.AtPosition(position);
    return meeple != null && meeple.IsHealthy();
  }

  /// <summary>
  /// Returns whether or not the given position on the board is empty.
  /// </summary>
  public static bool IsEmpty(this GameState state, DPosition position)
  {
    return state.AtPosition(position) == null;
    }
   /// <summary>
   /// Returns whether or not a particular piece is the highest priority
   /// </summary>
   public static int CheckRow()
    {
        return 0;
    }

    /// <summary>
    /// Returns whether or not a particular piece is the highest priority
    /// </summary>
    public static int CheckPriority()
    {
        return 0;
    }

    /// <summary>
    /// Returns whether or not a particular piece can reach the front
    /// </summary>
    public static int AccessToFront()
    {
        return 0;
    }
}
