using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for commands.
/// </summary>
public abstract class Command
{
    /// <summary>
    /// Client ID of the issuer.
    /// </summary>
    protected ulong _issuerID;

    public Command(ulong issuerID)
    {
        _issuerID = issuerID;
    }

    public abstract void Execute();
    public abstract void Reverse();
}
