using MLAPI;
using MLAPI.Logging;
using System;
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
            if (_currentState != value)
            {
                State prevState = _currentState; // MARK: Used for network logging
                if (value == State.PayForAction)
                {
                    CurrentlyPayingFor = _currentState;
                }
                _currentState = value;
                OnStateUpdate?.Invoke(value);
                // MARK: Update network state if connected, supports local debugging
                if ((NetworkManager.Singleton?.IsConnectedClient).GetValueOrDefault())
                {
                    NetworkLog.LogInfoServer($"Locally changed state ({prevState} -> {_currentState}).");
                    NetworkStateManager.Instance.NetworkCurrentState.Value = (int)value;
                }
            }
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