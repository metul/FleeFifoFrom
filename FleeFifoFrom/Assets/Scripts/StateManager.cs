using MLAPI;
using MLAPI.Logging;
using MLAPI.NetworkVariable;
using System;
using UnityEngine;

public class StateManager : NetworkBehaviour
{
    private static StateManager _instance;
    private static object _lock = new object();

    /// <summary>
    /// Singleton instance of the CommunicationManager.
    /// </summary>
    public static StateManager Instance
    {
        get
        {
            lock (_lock)
            {
                if (!_instance)
                {
                    _instance = (StateManager)FindObjectOfType(typeof(StateManager));
                    if (!_instance)
                    {
                        var singleton = new GameObject { name = "StateManager" };
                        _instance = singleton.AddComponent<StateManager>();
                        DontDestroyOnLoad(singleton);
                    }
                }
                return _instance;
            }
        }
    }

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

    public Action<State> OnStateUpdate;
    public State CurrentlyPayingFor;

    public State CurrentState
    {
        get => _currentState;
        set
        {
            if (_currentState != value)
            {
                if (value == State.PayForAction)
                {
                    CurrentlyPayingFor = _currentState;
                }
                NetworkLog.LogInfoServer($"Locally changing state ({_currentState} -> {value}) on client {NetworkManager.Singleton.LocalClientId}");
                _currentState = value;
                NetworkCurrentState.Value = (int)value;
                OnStateUpdate?.Invoke(value);
                // TODO: OnStateUpdate is null, possibly due to network object instantiation. Following lines are used for temporary debugging
                FindObjectOfType<FieldManager>().NetworkedUpdateInteractability();
                FindObjectOfType<FieldManager>().NetworkedUpdateInteractability();
                FindObjectOfType<DebugStateDisplay>().ModifyText(_currentState);
                FindObjectOfType<PriorityTileManager>().ToggleDisplay(_currentState);
            }
        }
    }

    private State _currentState;
    private NetworkVariableInt _networkCurrentState = new NetworkVariableInt(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.Everyone,
        SendTickrate = 0
    }, (int)State.Default);
    public NetworkVariableInt NetworkCurrentState => _networkCurrentState;

    private void OnEnable()
    {
        NetworkCurrentState.OnValueChanged += OnNetworkStateChanged;
    }

    private void OnDisable()
    {
        NetworkCurrentState.OnValueChanged -= OnNetworkStateChanged;
    }

    private void Start()
    {
        CurrentState = State.Default;
        GameState.Instance.OnTurnChange += types =>
        {
            CurrentState = State.Default;
        };
    }

    private void OnNetworkStateChanged(int prev, int next)
    {
        if (prev == next || next == (int)_currentState)
            return;
        NetworkLog.LogInfoServer($"NetworkStateChange invoked ({prev} -> {next}) on client {NetworkManager.Singleton.LocalClientId}");
        CurrentState = (State)next;
    }
}
