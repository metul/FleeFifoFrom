using MLAPI;
using MLAPI.Transports.UNET;
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
    private GameObject _waitingPanel;
    [SerializeField]
    private GameObject _waitingLog;
    [SerializeField]
    private UNetTransport _transport;

    private void Start()
    {
#if UNITY_SERVER
        InitializeServer();
#endif
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            _connectionPanel.SetActive(!_connectionPanel.activeSelf);
    }

    private void InitializeServer()
    {
        ServerEventListener sel = gameObject.AddComponent<ServerEventListener>();
        sel.Initialize(_waitingLog);
        NetworkManager.Singleton.StartServer();
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
        _waitingLog.SetActive(true);
    }

    public void UpdateConnectionAddress(string value)
    {
        if (string.IsNullOrEmpty(value))
            return;
        string[] splitAddress = value.Split(new char[] { ':' }, 2, System.StringSplitOptions.RemoveEmptyEntries);
        if (splitAddress.Length > 1)
        {
            string ip = splitAddress[0];
            if (IsValidIPv4(ip))
                _transport.ConnectAddress = ip;
            else
                throw new System.Exception($"Given IP address ({ip}) is invalid!");
            if (int.TryParse(splitAddress[1], out int port))
                _transport.ConnectPort = port;
            else
                throw new System.Exception($"Given port ({splitAddress[1]}) is invalid!");
        }
    }
}
