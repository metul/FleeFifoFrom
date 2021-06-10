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
        //S.R. Can we add RiotChooseFollowerType
        RiotChoosePath,
        Revive,
        Reprioritize,
        RetreatChooseTile,
        RetreatChooseKnight,
        Villager,
        MoveMeeple,
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

    private static State _currentState;
    public static Action<State> OnStateUpdate;
    public static State CurrentlyPayingFor;
    
    public static State CurrentState
    {
        get => _currentState;
        set
        {
            if (value == State.PayForAction)
            {
                CurrentlyPayingFor = _currentState;
            }
            _currentState = value;
            OnStateUpdate?.Invoke(value);
        } 
    }

    private void Start()
    {
        CurrentState = State.Default;
        GameState.Instance.OnTurnChange += types =>
        {
            CurrentState = State.Default;
        };
    }
}
