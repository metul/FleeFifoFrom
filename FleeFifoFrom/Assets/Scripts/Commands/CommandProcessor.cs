using System;
using System.Collections.Generic;

/// <summary>
/// Manager class for processing issued commands.
/// </summary>
public sealed class CommandProcessor
{
    static CommandProcessor() { }
    private CommandProcessor() { }

    public Action<bool> OnStackEmpty;

    /// <summary>
    /// Singleton instance of the CommandProcessor.
    /// </summary>
    public static CommandProcessor Instance { get; } = new CommandProcessor();

    /// <summary>
    /// Data structure for storing history of commands, used for undo/redo operations.
    /// </summary>
    private Stack<Command> _commands = new Stack<Command>();

    /// <summary>
    /// Executes command and saves into history.
    /// </summary>
    /// <param name="command"> Command to be executed. </param>
    public void ExecuteCommand(Command command)
    {
        var empty = _commands.Count == 0;
        if (command.IsFeasible())
        {
            _commands.Push(command);
            command.Execute();
        }

        if (empty)
            OnStackEmpty?.Invoke(false);
    }

    /// <summary>
    /// Undoes the most recent command in history.
    /// </summary>
    public void Undo()
    {
        _commands.Pop()?.Reverse();
        GameState.Instance.OnUndo?.Invoke();
        
        if (_commands.Count == 0)
            OnStackEmpty?.Invoke(true);
    }

    /// <summary>
    /// Clear command stack on turn rotation so the commands of the previous user are not undoable
    /// </summary>
    public void ClearStack()
    {
        _commands.Clear();
        OnStackEmpty?.Invoke(true);
    }
}
