using MLAPI;
using MLAPI.Logging;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Listener class for server events.
/// </summary>
public class ServerEventListener : MonoBehaviour
{
    private GameObject _logObject;

    public void Initialize(GameObject logObject)
    {
        _logObject = logObject;
        // TODO: Initialize other dependencies
    }

    private void OnEnable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }

    private void OnDisable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
    }

    /// <summary>
    /// Callback for client connection.
    /// </summary>
    /// <param name="obj"> ID for connected client. </param>
    private void OnClientConnected(ulong obj)
    {
        NetworkLog.LogInfoServer($"Client {obj} connected at timestamp {Time.time}.");
        NetworkLog.LogInfoServer($"There are a total of {NetworkManager.Singleton.ConnectedClients.Count} players connected.");
    }

    /// <summary>
    /// Callback for client disconnection.
    /// </summary>
    /// <param name="obj"> ID for disconnected client. </param>
    private void OnClientDisconnected(ulong obj)
    {
        NetworkLog.LogInfoServer($"Client {obj} disconnected at timestamp {Time.time}.");
        NetworkLog.LogInfoServer($"There are a total of {NetworkManager.Singleton.ConnectedClients.Count} players connected.");
    }

    /// <summary>
    /// Callback for server start.
    /// </summary>
    private void OnServerStarted()
    {
        NetworkLog.LogInfoServer($"Server started at timestamp {Time.time}.");
    }
}
