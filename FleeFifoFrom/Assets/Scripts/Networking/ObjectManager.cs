using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager class for registering DObjects, utilized for command synchronization.
/// </summary>
public sealed class ObjectManager
{
    static ObjectManager() { }

    private ObjectManager() { }

    /// <summary>
    /// The singleton instance of the ObjectManager.
    /// </summary>
    public static ObjectManager Instance { get; } = new ObjectManager();

    /// <summary>
    /// Dictionary storing object data.
    /// </summary>
    private Dictionary<ushort, DObject> _objectRegistry = new Dictionary<ushort, DObject>();

    /// <summary>
    /// Registers a local object.
    /// </summary>
    /// <param name="key"> Key for registering the object. </param>
    /// <param name="value"> Local object to store. </param>
    public void Register(ushort key, DObject value)
    {
        if (!_objectRegistry.ContainsKey(key))
            _objectRegistry.Add(key, value);
        else if (_objectRegistry[key] != value) // TODO: Probably need to override Equals() on DObject
            throw new Exception($"Another local object with the same ID {key} already registered!");
    }

    /// <summary>
    /// Deregisters a local object.
    /// </summary>
    /// <param name="key"> Key for deregistering the object. </param>
    public void Deregister(ushort key)
    {
        if (!_objectRegistry.Remove(key))
            throw new Exception($"No local object with the ID {key} is registered!");
    }

    /// <summary>
    /// Finds and returns an object if present.
    /// </summary>
    /// <param name="key"> Key to use for retrieval. </param>
    /// <returns> Registered local object. </returns>
    public DObject Request(ushort key)
    {
        if (!_objectRegistry.ContainsKey(key))
            throw new Exception($"No local object with the ID {key} is registered!");
        else
            return _objectRegistry[key];
    }
}
