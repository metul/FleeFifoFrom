using MLAPI;
using MLAPI.Logging;
using MLAPI.NetworkVariable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkStateManager : NetworkBehaviour
{
    private static NetworkStateManager _instance;
    private static object _lock = new object();

    /// <summary>
    /// Singleton instance of the NetworkStateManager.
    /// </summary>
    public static NetworkStateManager Instance
    {
        get
        {
            lock (_lock)
            {
                if (!_instance)
                {
                    _instance = (NetworkStateManager)FindObjectOfType(typeof(NetworkStateManager));
                    if (!_instance)
                    {
                        var singleton = new GameObject { name = "StateManager" };
                        _instance = singleton.AddComponent<NetworkStateManager>();
                        DontDestroyOnLoad(singleton);
                    }
                }
                return _instance;
            }
        }
    }

    private NetworkVariableInt _networkCurrentState = new NetworkVariableInt(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.Everyone,
        SendTickrate = 0
    }, (int)StateManager.State.Default);
    public NetworkVariableInt NetworkCurrentState => _networkCurrentState;

    private void OnEnable()
    {
        NetworkCurrentState.OnValueChanged += OnNetworkStateChanged;
    }

    private void OnDisable()
    {
        NetworkCurrentState.OnValueChanged -= OnNetworkStateChanged;
    }

    private void OnNetworkStateChanged(int prev, int next)
    {
        if (prev == next || next == (int)StateManager.CurrentState)
            return;
        NetworkLog.LogInfoServer($"NetworkStateChange invoked ({prev} -> {next}) on client {NetworkManager.Singleton.LocalClientId}");
        StateManager.CurrentState = (StateManager.State)next;
    }

    //public State CurrentState
    //{
    //    get => _currentState;
    //    set
    //    {
    //        if (_currentState != value)
    //        {
    //            if (value == State.PayForAction)
    //            {
    //                CurrentlyPayingFor = _currentState;
    //            }
    //            NetworkLog.LogInfoServer($"Locally changing state ({_currentState} -> {value}) on client {NetworkManager.Singleton.LocalClientId}");
    //            _currentState = value;
    //            NetworkCurrentState.Value = (int)value;
    //            OnStateUpdate?.Invoke(value);
    //            // TODO: OnStateUpdate is null, possibly due to network object instantiation. Following lines are used for temporary debugging
    //            FindObjectOfType<FieldManager>().NetworkedUpdateInteractability();
    //            FindObjectOfType<FieldManager>().NetworkedUpdateInteractability();
    //            FindObjectOfType<DebugStateDisplay>().ModifyText(_currentState);
    //            FindObjectOfType<PriorityTileManager>().ToggleDisplay(_currentState);
    //        }
    //    }
    //}
}
