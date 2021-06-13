using MLAPI;
using MLAPI.Logging;
using MLAPI.Messaging;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manager class for network communication.
/// </summary>
public class CommunicationManager : NetworkBehaviour
{
    private static CommunicationManager _instance;
    private static object _lock = new object();

    /// <summary>
    /// Singleton instance of the CommunicationManager.
    /// </summary>
    public static CommunicationManager Instance
    {
        get
        {
            lock (_lock)
            {
                if (!_instance)
                {
                    _instance = (CommunicationManager)FindObjectOfType(typeof(CommunicationManager));
                    if (!_instance)
                    {
                        var singleton = new GameObject { name = "CommunicationManager" };
                        _instance = singleton.AddComponent<CommunicationManager>();
                        DontDestroyOnLoad(singleton);
                    }
                }
                return _instance;
            }
        }
    }

    public void PublishAction(StateManager.State state)
    {
        if (!IsServer)
        {
            ulong publisherID = NetworkManager.Singleton.LocalClientId;
            NetworkLog.LogInfoServer($"Client {publisherID} has called the action {state}.");
            PublishActionServerRpc(publisherID, state);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PublishActionServerRpc(ulong publisherID, StateManager.State state)
    {
        IEnumerable<ulong> targetClientIds = NetworkManager.Singleton.ConnectedClients.Keys.ToArray().Except(new ulong[] { publisherID });
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = targetClientIds.ToArray() }
        };
        ProcessActionClientRpc(publisherID, state, clientRpcParams);
    }

    [ClientRpc]
    private void ProcessActionClientRpc(ulong publisherID, StateManager.State state, ClientRpcParams clientRpcParams = default)
    {
        // TODO: Process action locally
        Debug.Log($"Locally processing the action {state} issued by client {publisherID}.");
    }

    [ClientRpc]
    public void InitializeRandomSeedClientRpc(int seed)
    {
        Random.InitState(seed);
    }
}
