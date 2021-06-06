using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public enum State
    {
        Default,
        Authorize,
        Swap1,
        Swap2,
        Riot, // <- current debug state
        RiotChooseKnight,
        RiotChoosePath,
        Revive,
        Reprioritize,
        RetreatChooseTile,
        RetreatChooseKnight,
        Villager,
        ResetTurnSelect,
        ResetTurnMove,
        CountermandDrawCard,
        CountermandSelectCard,
        PoachSelectWorker,
        PoachSelectCard,
        Recall,
        Cooperate,
        PayForAction
    }

    private static State _gameState;
    // TODO: change this to an Observable<GameState> instead.
    public static Action OnStateUpdate;
    public static State CurrentlyPayingFor;
    
    public static State GameState
    {
        get => _gameState;
        set
        {
            if (value == State.PayForAction)
            {
                CurrentlyPayingFor = _gameState;
            }
            _gameState = value;
            OnStateUpdate?.Invoke();
        } 
    }

    private void Start()
    {
        GameState = State.Default;
    }
}
