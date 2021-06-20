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

    /// <summary>
    /// Initializes random with same (random) seed on each client.
    /// </summary>
    public void InitializeRandomSeed()
    {
        // MARK: sometimes throws IndexOutOfRangeException?
        InitializeRandomSeedClientRpc(Random.Range(int.MinValue, int.MaxValue));
    }

    [ClientRpc]
    private void InitializeRandomSeedClientRpc(int seed)
    {
        Random.InitState(seed);
    }

    /// <summary>
    /// Updates UI interactability by utilizing the corresponding ButtonManager method.
    /// </summary>
    public void UpdateInteractability()
    {
        UpdateInteractabilityClientRpc();
    }

    [ClientRpc]
    private void UpdateInteractabilityClientRpc()
    {
        FindObjectOfType<ButtonManager>().NetworkedUpdateInteractability(); // TODO: Use ButtonManager singleton instead?
    }

    /// <summary>
    /// Ends current turn and continues the game.
    /// </summary>
    public void RotateTurn()
    {
        RotateTurnServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RotateTurnServerRpc()
    {
        GameState.Instance.RotateTurn();
        RotateTurnClientRpc();
    }

    [ClientRpc]
    private void RotateTurnClientRpc()
    {
        GameState.Instance.RotateTurn();
    }

    public void RequestExecuteCommand(Command cmd)
    {
        switch (cmd)
        {
            case AuthorizeCommand authorizeCommand:
                ExecuteCommandServerRpc(authorizeCommand);
                break;
            case Countermand countermandCommand:
                ExecuteCommandServerRpc(countermandCommand);
                break;
            default:
                ExecuteCommandServerRpc(cmd);
                break;
        }
    }

    // Implement overloading methods for each command subtype since MLAPI RPCs reduce the subclass into the base class.
    // Another option is serializing a byte for type ID and create the subclass instance inside the RPC call by serializing the class data instead.
    #region Command Execution Overloads

    [ServerRpc(RequireOwnership = false)]
    private void ExecuteCommandServerRpc(Command cmd)
    {
        ExecuteCommand(cmd);
        ExecuteCommandClientRpc(cmd);
    }

    [ClientRpc]
    private void ExecuteCommandClientRpc(Command cmd)
    {
        ExecuteCommand(cmd);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ExecuteCommandServerRpc(AuthorizeCommand cmd)
    {
        ExecuteCommand(cmd);
        ExecuteCommandClientRpc(cmd);
    }

    [ClientRpc]
    private void ExecuteCommandClientRpc(AuthorizeCommand cmd)
    {
        ExecuteCommand(cmd);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ExecuteCommandServerRpc(Countermand cmd)
    {
        ExecuteCommand(cmd);
        ExecuteCommandClientRpc(cmd);
    }

    [ClientRpc]
    private void ExecuteCommandClientRpc(Countermand cmd)
    {
        ExecuteCommand(cmd);
    }

    #endregion

    private void ExecuteCommand(Command cmd)
    {
        CommandProcessor.Instance.Commands.Push(cmd);
        cmd.Execute();
    }

    public void RequestUndoCommand()
    {
        UndoCommandServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UndoCommandServerRpc()
    {
        UndoCommand();
        UndoCommandClientRpc();
    }

    [ClientRpc]
    private void UndoCommandClientRpc()
    {
        UndoCommand();
    }

    private void UndoCommand()
    {
        CommandProcessor.Instance.Commands.Pop()?.Reverse();
        GameState.Instance.OnUndo?.Invoke();
    }

    public void RequestClearCommandStack()
    {
        ClearCommandStackServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ClearCommandStackServerRpc()
    {
        ClearCommandStack();
        ClearCommandStackClientRpc();
    }

    [ClientRpc]
    private void ClearCommandStackClientRpc()
    {
        ClearCommandStack();
    }

    private void ClearCommandStack()
    {
        CommandProcessor.Instance.Commands.Clear();
    }

    #region Test

    public enum TesterType : byte
    {
        SerializationTesterSubclassA,
        SerializationTesterSubclassB
    }
    
    public void RequestTestExecution(byte type, SerializationTester test)
    {
        NetworkLog.LogInfoServer($"RequestTestExecution: {test.GetType()}");
        switch (test)
        {
            case SerializationTesterSubclassA stA:
                TestExecutionServerRpc(type, stA);
                break;
            case SerializationTesterSubclassB stB:
                TestExecutionServerRpc(type, stB);
                break;
            case SerializationTesterSubclassAuthorize stAuth:
                TestExecutionServerRpc(type, stAuth);
                break;
            default:
                TestExecutionServerRpc(type, test);
                break;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void TestExecutionServerRpc(byte type, SerializationTester test)
    {
        NetworkLog.LogInfoServer($"TestExecutionServerRpc: {test.GetType()}");
        TestExecution(type, test);
        TestExecutionClientRpc(type, test);
    }

    [ClientRpc]
    private void TestExecutionClientRpc(byte type, SerializationTester test)
    {
        NetworkLog.LogInfoServer($"TestExecutionClientRpc: {test.GetType()}");
        TestExecution(type, test);
    }

    [ServerRpc(RequireOwnership = false)]
    private void TestExecutionServerRpc(byte type, SerializationTesterSubclassA test)
    {
        NetworkLog.LogInfoServer($"TestExecutionServerRpc: {test.GetType()}");
        TestExecution(type, test);
        TestExecutionClientRpc(type, test);
    }

    [ClientRpc]
    private void TestExecutionClientRpc(byte type, SerializationTesterSubclassA test)
    {
        NetworkLog.LogInfoServer($"TestExecutionClientRpc: {test.GetType()}");
        TestExecution(type, test);
    }

    [ServerRpc(RequireOwnership = false)]
    private void TestExecutionServerRpc(byte type, SerializationTesterSubclassB test)
    {
        NetworkLog.LogInfoServer($"TestExecutionServerRpc: {test.GetType()}");
        TestExecution(type, test);
        TestExecutionClientRpc(type, test);
    }

    [ClientRpc]
    private void TestExecutionClientRpc(byte type, SerializationTesterSubclassB test)
    {
        NetworkLog.LogInfoServer($"TestExecutionClientRpc: {test.GetType()}");
        TestExecution(type, test);
    }

    [ServerRpc(RequireOwnership = false)]
    private void TestExecutionServerRpc(byte type, SerializationTesterSubclassAuthorize test)
    {
        NetworkLog.LogInfoServer($"TestExecutionServerRpc: {test.GetType()}");
        TestExecution(type, test);
        TestExecutionClientRpc(type, test);
    }

    [ClientRpc]
    private void TestExecutionClientRpc(byte type, SerializationTesterSubclassAuthorize test)
    {
        NetworkLog.LogInfoServer($"TestExecutionClientRpc: {test.GetType()}");
        TestExecution(type, test);
    }

    public void TestExecution(byte type, SerializationTester test)
    {
        NetworkLog.LogInfoServer($"TestExecution: {test.GetType()}");
        test.Execute();
    }

    #endregion
}
