using MLAPI;
using MLAPI.Logging;
using UnityEngine;

/// <summary>
/// Listener class for server events.
/// </summary>
public class ServerEventListener : MonoBehaviour
{
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
    }

    /// <summary>
    /// Callback for client disconnection.
    /// </summary>
    /// <param name="obj"> ID for disconnected client. </param>
    private void OnClientDisconnected(ulong obj)
    {
        NetworkLog.LogInfoServer($"Client {obj} disconnected at timestamp {Time.time}.");
    }

    /// <summary>
    /// Callback for server start.
    /// </summary>
    private void OnServerStarted()
    {
        NetworkLog.LogInfoServer($"Server started at timestamp {Time.time}.");
    }
}
