using MLAPI;
using System;
using System.Collections.Generic;
using RSG;

/// <summary>
/// Manager class for processing issued commands.
/// </summary>
public sealed class CommandProcessor
{
    static CommandProcessor() { }
    private CommandProcessor() { }

    public bool IsUndoable
    {
        get => _commands.Count > 0;
    }

    /// <summary>
    /// Singleton instance of the CommandProcessor.
    /// </summary>
    public static CommandProcessor Instance { get; } = new CommandProcessor();

    /// <summary>
    /// Data structure for storing history of commands, used for undo/redo operations.
    /// </summary>
    private Stack<Command> _commands = new Stack<Command>();
    public Stack<Command> Commands => _commands;

    /// <summary>
    /// Executes command and saves into history.
    /// </summary>
    /// <param name="command"> Command to be executed. </param>
    public IPromise ExecuteCommand(Command command)
    {
        var promise = new Promise();
        if (command.IsFeasible())
        {
            if ((NetworkManager.Singleton?.IsConnectedClient).GetValueOrDefault())
                CommunicationManager.Instance.RequestExecuteCommand(command);
            else // MARK: Allow local debugging
            {
                _commands.Push(command);
                command.Execute();
            }
            
            // TODO (Anas-Mert) figure out the moment this promise can be fulfilled
            promise.Resolve();
        }
        else
        {
            promise.Reject(new Exception("command not feasible"));
        }
        return promise;
    }

    /// <summary>
    /// Undoes the most recent command in history.
    /// </summary>
    public void Undo()
    {
        if ((NetworkManager.Singleton?.IsConnectedClient).GetValueOrDefault())
            CommunicationManager.Instance.RequestUndoCommand();
        else // MARK: Allow local debugging
        {
            _commands.Pop()?.Reverse();
            GameState.Instance.OnUndo?.Invoke();
        }
    }

    /// <summary>
    /// Clear command stack on turn rotation so the commands of the previous user are not undoable
    /// </summary>
    public void ClearStack()
    {
        if ((NetworkManager.Singleton?.IsConnectedClient).GetValueOrDefault())
            CommunicationManager.Instance.RequestClearCommandStack();
        else // MARK: Allow local debugging
            _commands.Clear();
    }
}
