using MLAPI;
using MLAPI.Logging;
using MLAPI.NetworkVariable;
using MLAPI.Serialization;
using MLAPI.Serialization.Pooled;
using System.IO;
using UnityEngine;

/// <summary>
/// Network counterpart of the FieldManager, handles corresponding network variables.
/// </summary>
public class NetworkFieldManager : NetworkBehaviour
{
    private static NetworkFieldManager _instance;
    private static object _lock = new object();

    /// <summary>
    /// Singleton instance of the NetworkFieldManager.
    /// </summary>
    public static NetworkFieldManager Instance
    {
        get
        {
            lock (_lock)
            {
                if (!_instance)
                {
                    _instance = (NetworkFieldManager)FindObjectOfType(typeof(NetworkFieldManager));
                    if (!_instance)
                    {
                        var singleton = new GameObject { name = "NetworkFieldManager" };
                        _instance = singleton.AddComponent<NetworkFieldManager>();
                        DontDestroyOnLoad(singleton);
                    }
                }
                return _instance;
            }
        }
    }

    private FieldManager _fieldManager;

    private void Awake()
    {
        _fieldManager = FindObjectOfType<FieldManager>(); // TODO (metul): Utilize singleton pattern instead?
    }

    private void OnEnable()
    {
        SerializationManager.RegisterSerializationHandlers<Tile>((Stream stream, Tile instance) =>
        {
            using (var writer = PooledNetworkWriter.Get(stream))
            {
                writer.WriteVector3Packed(instance.ID);
            }
        }, (Stream stream) =>
        {
            using (var reader = PooledNetworkReader.Get(stream))
            {
                return RegistryManager.Instance.Request(reader.ReadVector3Packed());
            }
        });
        NetworkStoreTile.OnValueChanged += OnNetworkStoreTileChanged;
        NetworkStoreSecondTile.OnValueChanged += OnNetworkStoreSecondTileChanged;
    }

    private void OnDisable()
    {
        SerializationManager.RemoveSerializationHandlers<Tile>();
        NetworkStoreTile.OnValueChanged -= OnNetworkStoreTileChanged;
        NetworkStoreSecondTile.OnValueChanged -= OnNetworkStoreSecondTileChanged;
    }

    private NetworkVariable<Tile> _networkStoreTile = new NetworkVariable<Tile>(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.Everyone,
        SendTickrate = 0
    }, null);
    public NetworkVariable<Tile> NetworkStoreTile => _networkStoreTile;

    private NetworkVariable<Tile> _networkStoreSecondTile = new NetworkVariable<Tile>(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.Everyone,
        SendTickrate = 0
    }, null);
    public NetworkVariable<Tile> NetworkStoreSecondTile => _networkStoreSecondTile;

    private void OnNetworkStoreTileChanged(Tile prev, Tile next)
    {
        if (prev == next || next == _fieldManager.StoreTile)
            return;
        //NetworkLog.LogInfoServer($"NetworkStoreTileChange invoked ({prev} -> {next}).");
        _fieldManager.StoreTile = next;
    }

    private void OnNetworkStoreSecondTileChanged(Tile prev, Tile next)
    {
        if (prev == next || next == _fieldManager.StoreSecondTile)
            return;
        //NetworkLog.LogInfoServer($"NetworkStoreSecondTileChange invoked ({prev} -> {next}).");
        _fieldManager.StoreSecondTile = next;
    }
}