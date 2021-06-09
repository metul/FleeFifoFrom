using MLAPI;
using MLAPI.NetworkVariable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager class for network players.
/// </summary>
public class PlayerManager : NetworkBehaviour
{
    private static PlayerManager _instance;
    private static object _lock = new object();

    /// <summary>
    /// Singleton instance of the CommunicationManager.
    /// </summary>
    public static PlayerManager Instance
    {
        get
        {
            lock (_lock)
            {
                if (!_instance)
                {
                    _instance = (PlayerManager)FindObjectOfType(typeof(PlayerManager));
                    if (!_instance)
                    {
                        var singleton = new GameObject { name = "PlayerManager" };
                        _instance = singleton.AddComponent<PlayerManager>();
                        DontDestroyOnLoad(singleton);
                    }
                }
                return _instance;
            }
        }
    }

    private NetworkVariableInt _playerCount = new NetworkVariableInt(0);
    public NetworkVariableInt PlayerCount => _playerCount;

    private void OnEnable()
    {
        PlayerCount.OnValueChanged += OnPlayerCountChanged;
    }

    private void OnDisable()
    {
        PlayerCount.OnValueChanged -= OnPlayerCountChanged;
    }

    private void OnPlayerCountChanged(int prev, int next)
    {
        // TODO
        Debug.Log($"Waiting for Players ({PlayerCount.Value}/4)");
        //_waitingLog.GetComponentInChildren<Text>().text = $"Waiting for Players ({_playerCount.Value}/4)";
    }
}
