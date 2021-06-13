using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private NetworkVariableInt _playerCount = new NetworkVariableInt(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly
    }, 0);
    public NetworkVariableInt PlayerCount => _playerCount;

    private NetworkDictionary<ulong, DPlayer.ID> _networkPlayerIDs = new NetworkDictionary<ulong, DPlayer.ID>(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.Everyone
    });
    public NetworkDictionary<ulong, DPlayer.ID> NetworkPlayerIDs => _networkPlayerIDs;

    private void OnEnable()
    {
        PlayerCount.OnValueChanged += OnPlayerCountChanged;
    }

    private void OnDisable()
    {
        PlayerCount.OnValueChanged -= OnPlayerCountChanged;
    }

    /// <summary>
    /// Callback for player count change.
    /// </summary>
    /// <param name="prev"> Player count before change. </param>
    /// <param name="next"> Player count after change. </param>
    private void OnPlayerCountChanged(int prev, int next)
    {
        // Handle initial invocation
        if (prev == next)
            return;
        // Map client ID to player ID
        if (!_networkPlayerIDs.ContainsKey(NetworkManager.Singleton.LocalClientId))
        {
            DPlayer.ID playerID = GameState.Instance.Players[PlayerCount.Value - 1].Id;
            _networkPlayerIDs.Add(NetworkManager.Singleton.LocalClientId, playerID);
            // Set player indicator color on UI
            ConnectionManager.Instance.SetPlayerIndicatorColor(ColorUtils.GetPlayerColor(playerID));
        }
        // Update text
        int requiredPlayers = GameState.Instance.Players.Length;
        ConnectionManager.Instance.ModifyWaitingText(PlayerCount.Value, requiredPlayers);
        // Initialize/interrupt game
        if (PlayerCount.Value == requiredPlayers)
            StartCoroutine(StartGame());
        else if (prev == requiredPlayers && next < prev)
            StopGame();
    }

    /// <summary>
    /// Initializes the game.
    /// </summary>
    private IEnumerator StartGame()
    {
        // Initialize random with same (random) seed for each client
        if (IsServer)
            CommunicationManager.Instance.InitializeRandomSeedClientRpc(Random.Range(int.MinValue, int.MaxValue));
        // TODO: Wait until random seeded
        yield return new WaitForSeconds(3);
        // Set up the board
        GameState.Instance.DrawMeeple();
        // Disable connection UI
        ConnectionManager.Instance.DisableUI();
    }

    /// <summary>
    /// Interrupts the game.
    /// </summary>
    private void StopGame()
    {
        Debug.Log($"Player count dropped to {PlayerCount.Value} after connection loss, interrupting game...");
        // TODO
    }
}
