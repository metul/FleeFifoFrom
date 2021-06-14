using MLAPI;
using MLAPI.Logging;
using MLAPI.NetworkVariable;
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

    private static NetworkVariable<State> _currentState = new NetworkVariable<State>(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.Everyone
    }, State.Default);
    public static Action<State> OnStateUpdate;
    public static NetworkVariable<State> CurrentlyPayingFor;

    public static NetworkVariable<State> CurrentState => _currentState;

    private void OnEnable()
    {
        CurrentState.OnValueChanged += OnStateChanged;
    }

    private void OnDisable()
    {
        CurrentState.OnValueChanged -= OnStateChanged;
    }

    private void Start()
    {
        CurrentState.Value = State.Default;
        GameState.Instance.OnTurnChange += types =>
        {
            CurrentState.Value = State.Default;
        };
    }

    private void OnStateChanged(State prevState, State nextState)
    {
        if (NetworkManager.Singleton.IsConnectedClient)
            NetworkLog.LogInfoServer($"OnStateChanged - {NetworkManager.Singleton.LocalClientId}: {prevState} - {nextState}");
        else
            Debug.Log($"OnStateChanged - : {prevState} - {nextState}");
        if (nextState == State.PayForAction)
        {
            CurrentlyPayingFor.Value = prevState;
        }
        OnStateUpdate?.Invoke(nextState);
    }
}
