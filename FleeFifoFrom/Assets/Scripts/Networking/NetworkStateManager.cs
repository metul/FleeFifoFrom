using MLAPI;
using MLAPI.Logging;
using MLAPI.NetworkVariable;
using UnityEngine;

/// <summary>
/// Network counterpart of the StateManager, handles corresponding network variables.
/// </summary>
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
                        var singleton = new GameObject { name = "NetworkStateManager" };
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
        if (prev == next || next == (int)StateManager.CurrentState) // TODO (metul): Might cause error for StateX -> StateX transitions (riot step)
            return;
        NetworkLog.LogInfoServer($"NetworkStateChange invoked ({prev} -> {next}).");
        StateManager.CurrentState = (StateManager.State)next;
    }
}
