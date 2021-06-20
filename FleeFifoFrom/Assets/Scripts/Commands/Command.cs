using UnityEngine;
using MLAPI.Serialization;

/// <summary>
/// Base class for commands.
/// </summary>
public abstract class Command : INetworkSerializable
{
    /// <summary>
    /// Client ID of the issuer.
    /// </summary>
    protected ulong _issuerID;

    // Default constructor needed for serialization
    public Command() : base() { }

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

    public abstract bool IsFeasible();

    public virtual void NetworkSerialize(NetworkSerializer serializer)
    {
        serializer.Serialize(ref _issuerID);
    }
}
