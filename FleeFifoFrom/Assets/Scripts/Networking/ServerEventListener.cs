using MLAPI;
using MLAPI.Logging;
using UnityEngine;

/// <summary>
/// Listener class for server events.
/// </summary>
public class ServerEventListener : MonoBehaviour
{
    private PlayerManager _playerManager;

    private void Awake()
    {
        _playerManager = PlayerManager.Instance;
    }

    private void OnEnable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        _playerManager.OnRequiredPlayersReached += CommunicationManager.Instance.InitializeRandomSeed;
        _playerManager.OnRequiredPlayersReached += CommunicationManager.Instance.UpdateInteractability;
    }

    private void OnDisable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
        _playerManager.OnRequiredPlayersReached -= CommunicationManager.Instance.InitializeRandomSeed;
        _playerManager.OnRequiredPlayersReached -= CommunicationManager.Instance.UpdateInteractability;
    }

    /// <summary>
    /// Callback for client connection.
    /// </summary>
    /// <param name="clientID"> ID for connected client. </param>
    private void OnClientConnected(ulong clientID)
    {
        NetworkLog.LogInfoServer($"Client {clientID} connected at timestamp {Time.time}.");
        NetworkLog.LogInfoServer($"There are a total of {NetworkManager.Singleton.ConnectedClients.Count} players connected.");
        // Map client ID to player ID
        if (!_playerManager.NetworkPlayerIDs.ContainsKey(clientID))
        {
            int requiredPlayerCount = GameState.Instance.Players.Length;
            // Check if max players reached
            if (NetworkManager.Singleton.ConnectedClients.Count > requiredPlayerCount)
                throw new System.Exception($"Required number of players ({requiredPlayerCount}) already reached, can't connect further clients!");
            // Register new player
            DPlayer.ID playerID = GameState.Instance.Players[_playerManager.PlayerCount.Value].Id;
            NetworkLog.LogInfoServer($"Mapping client ID {clientID} to player ID {playerID}.");
            _playerManager.NetworkPlayerIDs.Add(clientID, playerID);
            _playerManager.PlayerCount.Value++;
            NetworkCommandProcessor.Instance.CommandExecutionRegistry.Add(clientID, false);
        }
    }

    /// <summary>
    /// Callback for client disconnection.
    /// </summary>
    /// <param name="clientID"> ID for disconnected client. </param>
    private void OnClientDisconnected(ulong clientID)
    {
        NetworkLog.LogInfoServer($"Client {clientID} disconnected at timestamp {Time.time}.");
        NetworkLog.LogInfoServer($"There are a total of {NetworkManager.Singleton.ConnectedClients.Count} players connected.");
        // Remove client/player ID pair
        if (_playerManager.NetworkPlayerIDs.ContainsKey(clientID))
        {
            NetworkLog.LogInfoServer($"Removing client ID {clientID} from dictionary.");
            _playerManager.NetworkPlayerIDs.Remove(clientID);
            _playerManager.PlayerCount.Value--;
        }
    }

    /// <summary>
    /// Callback for server start.
    /// </summary>
    private void OnServerStarted()
    {
        NetworkLog.LogInfoServer($"Server started at timestamp {Time.time}.");
    }
}
