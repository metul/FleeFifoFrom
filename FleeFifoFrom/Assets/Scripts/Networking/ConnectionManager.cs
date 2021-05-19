using MLAPI;
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
        
    }

    private void Update()
    {
        // MARK: Used for debug purposes
        if (Input.GetKeyDown(KeyCode.F1))
            _connectionPanel.SetActive(!_connectionPanel.activeSelf);
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

    private bool IsValidIPv4(string ipString)
    {
        return ipString.Count(c => c == '.') == 3 && IPAddress.TryParse(ipString, out IPAddress iPAddress);
    }
}
