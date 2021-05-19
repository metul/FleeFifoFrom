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
    public static Action OnUpdateInteractability;
    public static State GameState
    {
        get => _gameState;
        set
        {
            _gameState = value;
            OnUpdateInteractability?.Invoke();
        } 
    }
}
