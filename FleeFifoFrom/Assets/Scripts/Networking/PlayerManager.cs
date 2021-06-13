using MLAPI;
using MLAPI.Logging;
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

    /// <summary>
    /// Callback for player count change.
    /// </summary>
    /// <param name="prev"> Player count before change. </param>
    /// <param name="next"> Player count after change. </param>
    private void OnPlayerCountChanged(int prev, int next)
    {
        int requiredPlayers = GameState.Instance.Players.Length;
        ConnectionManager.Instance.ModifyWaitingText(PlayerCount.Value, requiredPlayers);
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
        NetworkLog.LogInfoServer($"Client {NetworkManager.LocalClientId} has random sequence " +
            $"{Random.Range(1, 10)}-{Random.Range(1, 10)}-{Random.Range(1, 10)}");
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
