using MLAPI;
using MLAPI.Logging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using System;
using UnityEngine;

/// <summary>
/// Network counterpart of the CommandProcessor, handles corresponding network variables.
/// </summary>
public class NetworkCommandProcessor : NetworkBehaviour
{
    private static NetworkCommandProcessor _instance;
    private static object _lock = new object();

    /// <summary>
    /// Singleton instance of the NetworkStateManager.
    /// </summary>
    public static NetworkCommandProcessor Instance
    {
        get
        {
            lock (_lock)
            {
                if (!_instance)
                {
                    _instance = (NetworkCommandProcessor)FindObjectOfType(typeof(NetworkCommandProcessor));
                    if (!_instance)
                    {
                        var singleton = new GameObject { name = "NetworkCommandProcessor" };
                        _instance = singleton.AddComponent<NetworkCommandProcessor>();
                        DontDestroyOnLoad(singleton);
                    }
                }
                return _instance;
            }
        }
    }

    private NetworkVariableInt _commandExecutionCount = new NetworkVariableInt(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.Everyone,
        SendTickrate = 0
    }, 0);
    public NetworkVariableInt CommandExecutionCount => _commandExecutionCount; // TODO: Command execution count can not be updated synchronously (e.g. multiple clients update value (0 -> 1)

    private NetworkDictionary<ulong, bool> _commandExecutionRegistry = new NetworkDictionary<ulong, bool>(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.Everyone,
        SendTickrate = 0
    });
    public NetworkDictionary<ulong, bool> CommandExecutionRegistry => _commandExecutionRegistry;

    private void OnEnable()
    {
        CommandExecutionCount.OnValueChanged += OnCommandExecutionCountChanged;
        CommandExecutionRegistry.OnDictionaryChanged += OnCommandExecutionRegistered;
    }

    private void OnDisable()
    {
        CommandExecutionCount.OnValueChanged -= OnCommandExecutionCountChanged;
        CommandExecutionRegistry.OnDictionaryChanged -= OnCommandExecutionRegistered;
    }

    private void OnCommandExecutionCountChanged(int prev, int next)
    {
        if (prev == next)
            return;
        NetworkLog.LogInfoServer($"Execution count change: ({prev} -> {next}).");
    }

    private void OnCommandExecutionRegistered(NetworkDictionaryEvent<ulong, bool> changeEvent)
    {
        NetworkLog.LogInfoServer($"Command execution registered: ({changeEvent.Key} / {changeEvent.Value}).");
    }

    /// <summary>
    /// Registers client specific command execution as done.
    /// </summary>
    /// <param name="key"></param>
    public void RegisterCommand(ulong key)
    {
        if (_commandExecutionRegistry.ContainsKey(key))
            _commandExecutionRegistry[key] = true;
        else
            throw new Exception($"No client with ID {key} is registered in command execution registry!");
    }

    /// <summary>
    /// Resets all flags for command execution.
    /// </summary>
    public void ResetCommandExecutionRegistry()
    {
        foreach (ulong key in _commandExecutionRegistry.Keys)
            _commandExecutionRegistry[key] = false;
    }

    /// <summary>
    /// Checks whether all clients executed the command.
    /// </summary>
    /// <returns></returns>
    public bool IsCommandExecuted()
    {
        foreach (var value in _commandExecutionRegistry.Values)
            if (!value)
                return false;
        return true;
    }
}
