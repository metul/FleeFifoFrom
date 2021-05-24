using MLAPI;
using MLAPI.Logging;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manager class for network communication.
/// </summary>
public class CommunicationManager : NetworkBehaviour
{
    /// <summary>
    /// Singleton instance of the CommunicationManager.
    /// </summary>
    public static CommunicationManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void PublishAction(FieldManager.DebugGameState state)
    {
        if (!IsServer)
        {
            ulong publisherID = NetworkManager.Singleton.LocalClientId;
            NetworkLog.LogInfoServer($"Client {publisherID} has called the action {state}.");
            PublishActionServerRPC(publisherID, state);
        }
    }

    [ServerRpc (RequireOwnership = false)]
    private void PublishActionServerRPC(ulong publisherID, FieldManager.DebugGameState state)
    {
        IEnumerable<ulong> targetClientIds = NetworkManager.Singleton.ConnectedClients.Keys.ToArray().Except(new ulong[] { publisherID });
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = targetClientIds.ToArray() }
        };
        ProcessActionClientRPC(publisherID, state, clientRpcParams);
    }

    [ClientRpc]
    private void ProcessActionClientRPC(ulong publisherID, FieldManager.DebugGameState state, ClientRpcParams clientRpcParams = default)
    {
        // TODO: Process action locally
        Debug.Log($"Locally processing the action {state} issued by client {publisherID}.");
    }
}
