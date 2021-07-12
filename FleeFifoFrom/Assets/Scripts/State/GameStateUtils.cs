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
  /// Will returns all meeple at position matching given criteria
  /// </summary>
  public static DMeeple[] AllAtPositions(
    this GameState state,
    DPosition[] positions,
    System.Func<DMeeple, bool> check
  )
  {
    return state.Meeple.Where(m => (
        m.State == DMeeple.MeepleState.InQueue
        && m.Position.Current != null && Array.Exists(positions, p => p.Equals(m.Position.Current))
        && check(m)
    )).ToArray();
  }

  public static DMeeple[] AllAtPosition(
    this GameState state,
    DPosition position,
    System.Func<DMeeple, bool> check
  )
  {
    return state.AllAtPositions(new DPosition[]{ position }, check);
  }

  public static DMeeple[] AllAtPosition(this GameState state, DPosition position)
  {
    return state.AllAtPosition(position, m => true);
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

   public static DKnight[] AuthorizedKnights(this GameState state, DPlayer.ID player)
   {
     return state.Knights.Where(v => v.State == DMeeple.MeepleState.Authorized && v.Owner == player).ToArray();
   }

    /// <summary>
    /// Will return all workers available to a player (including those whom they have poached).
    /// </summary>
    public static DWorker[] AvailableWorkers(this GameState state, DPlayer.ID player)
  {
    return state.Workers.Where(w => w.State == DWorker.WorkerState.InPool && w.ControlledBy == player).ToArray();
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

  /// <summary>
  /// Will return true if a path from given initial positions
  /// to given endpoint exists, where every step also matches given condition.
  /// <br/>
  /// 👉 checks the condition on initial positions as well. <br/>
  /// 👉 DOES NOT check the condition on final position.
  /// </summary>
  public static bool PathExists(
    this GameState state,
    DPosition[] start,
    DPosition target,
    System.Func<DPosition, bool> check)
  {
    IEnumerable<DPosition> current = start.Where(check);
    while (current.Count() > 0)
    {
      List<DPosition> next = new List<DPosition>();
      foreach (var pos in current)
      {
        if (pos.Equals(target))
          return true;

        foreach (var succ in pos.Successors())
        {
          if (succ.Equals(target))
            return true;

          if (check(succ))
          {
            next.Add(succ);
          }
        }
      }

      current = next;
    }

    return false;
  }

  /// <summary>
  /// Will return true if a path from given initial position
  /// to given endpoint exists, where every step also matches given condition.
  /// <br/>
  /// 👉 checks the condition on initial position as well. <br/>
  /// 👉 DOES NOT check the condition on final position.
  /// </summary>
  public static bool PathExists(
    this GameState state,
    DPosition start,
    DPosition target,
    System.Func<DPosition, bool> check
  )
  {
    return state.PathExists(new DPosition[]{ start }, target, check);
  }

  public static bool IsInjured(this DMeeple meeple)
  {
    return (
      meeple.GetType().IsSubclassOf(typeof(DVillager))
      && ((DVillager) meeple).Health.Current == DVillager.HealthStates.Injured
    );
  }

  public static bool IsHealthy(this DMeeple meeple)
  {
    return (
      !meeple.GetType().IsSubclassOf(typeof(DVillager))
      || ((DVillager) meeple).Health.Current == DVillager.HealthStates.Healthy
    );
  }

  /// <summary>
  /// Returns true if there is an injured villager at given position
  /// </summary>
  public static bool InjuredVillagerAtPosition(this GameState state, DPosition position)
  {
    return state.AllAtPosition(position, m => m.IsInjured()).Length > 0;
  }

  /// <summary>
  /// Returns true if there is a healthy meeple at given position
  /// </summary>
  public static bool HealthyMeepleAtPosition(this GameState state, DPosition position)
  {
    return state.AllAtPosition(position, m => m.IsHealthy()).Length > 0;
  }

  public static DKnight[] Vanguard(this GameState state)
  {
    return state.Knights.Where(knight => knight.State == DMeeple.MeepleState.OutOfBoard).ToArray();
  }

  // TODO: complete these
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
    public static bool CheckPriority(DMeeple current)
    {
        /*bool result = true
        current.Priority is the priority of the current meeple
        for each meeple in this row
          { 
            if (meeple.priority > current.Priority)
            {
                return false
            }
           }

        return result;*/
        return true; //remove this
    }
    public static bool CanEndTurn(this GameState state)
    {
        // always true for action turns or when no villager left to draw
        if (state.TurnType == GameState.TurnTypes.ActionTurn || state.VillagerBagCount.Current == 0)
        {
            return true;
        }

        // villager left: check for empty tiles that can be got to from last row without overpassing
        // an injured villager
        var valid = true;
        state.TraverseBoard(p =>
        {
            if (state.IsEmpty(p) && 
              state.PathExists(DPosition.LastRow(), p, _p => !state.InjuredVillagerAtPosition(_p))) {
              valid = false;
            }
        });
        return valid;
    }

    /// <summary>
    /// Obj Helper functions
    /// </summary>
    /// 
    //Count how many pieces of each type a player has
    //Used for Rescue and Admin cards
    /*
public static int CountType(PlayerID player)
{
    {
        int scholar = 0;
        int merchant = 0;
        int farmer = 0;
        for each authorized villager in player inventory
        {
            if (piece.Type is Scholar)
            {
                scholar++;
            }
            elseif(piece.Type is Merchant)
            {
                merchant++;
            }
        else
        {
                farmer++;
            }
        }

        return [scholar, merchant, farmer];
        //I think we would need a data structure similar to the priority matrix itself instead
    }

}
*/

    /// <summary>
    /// Counts how many pieces of each priority a player has
    /// Used for rescue cards
    /// </summary>
    /*
        public static int[] CountPriority(PlayerID player)
    {   
        int[] result = [0,0,0]; //Im fairly certain this is an incorrect array definition tho
        int low = 0;
        int med = 0;
        int high = 0;
        for each authorized villager in player inventory
        {
            if(piece.Priority == 2)
            { 
                high++;
            }
            elseif (piece.Priority == 1)
            {
                med++;
            }
            else
            {
                low++;
            }
        }
        
        return [low,med,high];
        //I think we would need a data structurure similar to the priority matrix itself instead
    }*/


    //TODO: The following function could be a common parent for each objective card
    /*
    public static int CheckObjective()
    {
       for each condition
            Check condition
            if all conditions
                    return true
    }
    */

    //TODO: Would need to have a separate definiton of this
    //or separate case types for
    //CheckShape, CheckCount, CheckInjury, CheckHonor
    public static int CheckCondition()
    {
        return 0;
    }

    public static int CheckShape()
    {
        return 0;
    }
}
