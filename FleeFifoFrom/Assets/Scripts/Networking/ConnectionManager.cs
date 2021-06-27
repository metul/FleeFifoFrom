using MLAPI;
using MLAPI.Transports.UNET;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField]
    private Image _playerIndicatorImage;

    /// <summary>
    /// Singleton instance of the ConnectionManager.
    /// </summary>
    public static ConnectionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
#if UNITY_SERVER
        InitializeServer();
#endif
    }

    private void InitializeServer()
    {
        gameObject.AddComponent<ServerEventListener>();
        NetworkManager.Singleton.StartServer();
    }

    private bool IsValidIPv4(string ipString)
    {
        return ipString.Count(c => c == '.') == 3 && IPAddress.TryParse(ipString, out IPAddress iPAddress);
    }

    public void StartServer()
    {
        gameObject.AddComponent<ServerEventListener>();
        NetworkManager.Singleton.StartServer();
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

    public void ModifyWaitingText(int playerCount, int requiredPlayers)
    {
        _waitingLog.GetComponentInChildren<Text>().text = $"Waiting for Players ({playerCount}/{requiredPlayers})";
    }

    public void DisableUI()
    {
        _connectionPanel.SetActive(false);
        _waitingPanel.SetActive(false);
        _waitingLog.SetActive(false);
        // Set player indicator color on UI
        if (!NetworkManager.Singleton.IsServer)
            _playerIndicatorImage.color = ColorUtils.GetPlayerColor(PlayerManager.Instance.NetworkPlayerIDs[NetworkManager.Singleton.LocalClientId]);
    }
}
