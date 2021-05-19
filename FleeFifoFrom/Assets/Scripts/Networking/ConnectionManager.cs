using MLAPI;
using MLAPI.Logging;
using MLAPI.Transports.UNET;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

/// <summary>
/// Manager class for server connection and UI.
/// </summary>
public class ConnectionManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _connectionPanel;
    [SerializeField]
    private UNetTransport _transport;

    private void Start()
    {
#if UNITY_SERVER
        NetworkManager.Singleton.StartServer();
#endif
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            _connectionPanel.SetActive(!_connectionPanel.activeSelf);
    }

    private void OnEnable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnDisable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
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

    private bool IsValidIPv4(string ipString)
    {
        return ipString.Count(c => c == '.') == 3 && IPAddress.TryParse(ipString, out IPAddress iPAddress);
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        _connectionPanel.SetActive(false);
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        _connectionPanel.SetActive(false);
    }

    public void UpdateIP(string value)
    {
        if (!string.IsNullOrEmpty(value) && IsValidIPv4(value))
            _transport.ConnectAddress = value;
    }
}
