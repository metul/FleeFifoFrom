using MLAPI;
using MLAPI.Messaging;
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

    /// <summary>
    /// Initializes random with same (random) seed on each client.
    /// </summary>
    public void InitializeRandomSeed()
    {
        // MARK: sometimes throws IndexOutOfRangeException?
        int randomSeed = Random.Range(int.MinValue, int.MaxValue);
        Random.InitState(randomSeed);
        InitializeRandomSeedClientRpc(randomSeed);
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
        FindObjectOfType<ButtonManager>().NetworkedUpdateInteractability(); // TODO: Use ButtonManager singleton instead?
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
            case ObjectiveCommand objectiveCommand:
                ExecuteCommandServerRpc(objectiveCommand);
                break;
            case ReviveCommand reviveCommand:
                ExecuteCommandServerRpc(reviveCommand);
                break;
            case StartRiotCommand startRiotCommand:
                ExecuteCommandServerRpc(startRiotCommand);
                break;
            case RiotStepCommand riotStepCommand:
                ExecuteCommandServerRpc(riotStepCommand);
                break;
            case RiotAuthorizeCommand riotAuthorizeCommand:
                ExecuteCommandServerRpc(riotAuthorizeCommand);
                break;
            case SwapCommand swapCommand:
                ExecuteCommandServerRpc(swapCommand);
                break;
            case CooperateCommand cooperateCommand:
                ExecuteCommandServerRpc(cooperateCommand);
                break;
            case Countermand countermandCommand:
                ExecuteCommandServerRpc(countermandCommand);
                break;
            case DrawVillagerCommand drawVillagerCommand:
                ExecuteCommandServerRpc(drawVillagerCommand);
                break;
            case MoveVillagerCommand moveVillagerCommand:
                ExecuteCommandServerRpc(moveVillagerCommand);
                break;
            case PoachCommand poachCommand:
                ExecuteCommandServerRpc(poachCommand);
                break;
            case RecallCommand recallCommand:
                ExecuteCommandServerRpc(recallCommand);
                break;
            case ReprioritizeCommand reprioritizeCommand:
                ExecuteCommandServerRpc(reprioritizeCommand);
                break;
            case RetreatCommand retreatCommand:
                ExecuteCommandServerRpc(retreatCommand);
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
    private void ExecuteCommandServerRpc(ObjectiveCommand cmd)
    {
        ExecuteCommand(cmd);
        ExecuteCommandClientRpc(cmd);
    }

    [ClientRpc]
    private void ExecuteCommandClientRpc(ObjectiveCommand cmd)
    {
        ExecuteCommand(cmd);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ExecuteCommandServerRpc(ReviveCommand cmd)
    {
        ExecuteCommand(cmd);
        ExecuteCommandClientRpc(cmd);
    }

    [ClientRpc]
    private void ExecuteCommandClientRpc(ReviveCommand cmd)
    {
        ExecuteCommand(cmd);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void ExecuteCommandServerRpc(StartRiotCommand cmd)
    {
        ExecuteCommand(cmd);
        ExecuteCommandClientRpc(cmd);
    }

    [ClientRpc]
    private void ExecuteCommandClientRpc(StartRiotCommand cmd)
    {
        ExecuteCommand(cmd);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ExecuteCommandServerRpc(RiotStepCommand cmd)
    {
        ExecuteCommand(cmd);
        ExecuteCommandClientRpc(cmd);
    }

    [ClientRpc]
    private void ExecuteCommandClientRpc(RiotStepCommand cmd)
    {
        ExecuteCommand(cmd);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void ExecuteCommandServerRpc(RiotAuthorizeCommand cmd)
    {
        ExecuteCommand(cmd);
        ExecuteCommandClientRpc(cmd);
    }

    [ClientRpc]
    private void ExecuteCommandClientRpc(RiotAuthorizeCommand cmd)
    {
        ExecuteCommand(cmd);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ExecuteCommandServerRpc(SwapCommand cmd)
    {
        ExecuteCommand(cmd);
        ExecuteCommandClientRpc(cmd);
    }

    [ClientRpc]
    private void ExecuteCommandClientRpc(SwapCommand cmd)
    {
        ExecuteCommand(cmd);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ExecuteCommandServerRpc(CooperateCommand cmd)
    {
        ExecuteCommand(cmd);
        ExecuteCommandClientRpc(cmd);
    }

    [ClientRpc]
    private void ExecuteCommandClientRpc(CooperateCommand cmd)
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

    [ServerRpc(RequireOwnership = false)]
    private void ExecuteCommandServerRpc(DrawVillagerCommand cmd)
    {
        ExecuteCommand(cmd);
        ExecuteCommandClientRpc(cmd);
    }

    [ClientRpc]
    private void ExecuteCommandClientRpc(DrawVillagerCommand cmd)
    {
        ExecuteCommand(cmd);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ExecuteCommandServerRpc(MoveVillagerCommand cmd)
    {
        ExecuteCommand(cmd);
        ExecuteCommandClientRpc(cmd);
    }

    [ClientRpc]
    private void ExecuteCommandClientRpc(MoveVillagerCommand cmd)
    {
        ExecuteCommand(cmd);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ExecuteCommandServerRpc(PoachCommand cmd)
    {
        ExecuteCommand(cmd);
        ExecuteCommandClientRpc(cmd);
    }

    [ClientRpc]
    private void ExecuteCommandClientRpc(PoachCommand cmd)
    {
        ExecuteCommand(cmd);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ExecuteCommandServerRpc(RecallCommand cmd)
    {
        ExecuteCommand(cmd);
        ExecuteCommandClientRpc(cmd);
    }

    [ClientRpc]
    private void ExecuteCommandClientRpc(RecallCommand cmd)
    {
        ExecuteCommand(cmd);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ExecuteCommandServerRpc(ReprioritizeCommand cmd)
    {
        ExecuteCommand(cmd);
        ExecuteCommandClientRpc(cmd);
    }

    [ClientRpc]
    private void ExecuteCommandClientRpc(ReprioritizeCommand cmd)
    {
        ExecuteCommand(cmd);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ExecuteCommandServerRpc(RetreatCommand cmd)
    {
        ExecuteCommand(cmd);
        ExecuteCommandClientRpc(cmd);
    }

    [ClientRpc]
    private void ExecuteCommandClientRpc(RetreatCommand cmd)
    {
        ExecuteCommand(cmd);
    }

    #endregion

    private void ExecuteCommand(Command cmd)
    {
        CommandProcessor.Instance.Commands.Push(cmd);
        cmd.Execute();
        if ((NetworkManager.Singleton?.IsConnectedClient).GetValueOrDefault())
            NetworkCommandProcessor.Instance.RegisterCommand(NetworkManager.Singleton.LocalClientId);
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
}
