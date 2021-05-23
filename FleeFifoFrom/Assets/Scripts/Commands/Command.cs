using UnityEngine;

/// <summary>
/// Base class for commands.
/// </summary>
public abstract class Command : MonoBehaviour
{
    /// <summary>
    /// Client ID of the issuer.
    /// </summary>
    protected ulong _issuerID;

    public Command(ulong issuerID)
    {
        _issuerID = issuerID;
    }

    /// <summary>
    /// Executes the issued command.
    /// </summary>
    public abstract void Execute();

    /// <summary>
    /// Reverses the issued command.
    /// </summary>
    public abstract void Reverse();

    public abstract bool IsFeasibile();
}
