using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager class for registering DObjects, DPlayers and Tiles, utilized for command synchronization.
/// </summary>
public sealed class RegistryManager
{
    static RegistryManager() { }

    private RegistryManager() { }

    /// <summary>
    /// The singleton instance of the RegistryManager.
    /// </summary>
    public static RegistryManager Instance { get; } = new RegistryManager();

    /// <summary>
    /// Dictionary storing object data.
    /// </summary>
    private Dictionary<ushort, DObject> _objectRegistry = new Dictionary<ushort, DObject>();

    /// <summary>
    /// Dictionary storing object data.
    /// </summary>
    private Dictionary<DPlayer.ID, DPlayer> _playerRegistry = new Dictionary<DPlayer.ID, DPlayer>();

    /// <summary>
    /// Dictionary storing object data.
    /// </summary>
    private Dictionary<Vector3, Tile> _tileRegistry = new Dictionary<Vector3, Tile>();

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

    /// <summary>
    /// Registers a local player.
    /// </summary>
    /// <param name="key"> Key for registering the player. </param>
    /// <param name="value"> Local player to store. </param>
    public void Register(DPlayer.ID key, DPlayer value)
    {
        if (!_playerRegistry.ContainsKey(key))
            _playerRegistry.Add(key, value);
        else if (_playerRegistry[key] != value)
            throw new Exception($"Another local player with the same ID {key} already registered!");
    }

    /// <summary>
    /// Deregisters a local player.
    /// </summary>
    /// <param name="key"> Key for deregistering the player. </param>
    public void Deregister(DPlayer.ID key)
    {
        if (!_playerRegistry.Remove(key))
            throw new Exception($"No local player with the ID {key} is registered!");
    }

    /// <summary>
    /// Finds and returns an player if present.
    /// </summary>
    /// <param name="key"> Key to use for retrieval. </param>
    /// <returns> Registered local player. </returns>
    public DPlayer Request(DPlayer.ID key)
    {
        if (!_playerRegistry.ContainsKey(key))
            throw new Exception($"No local player with the ID {key} is registered!");
        else
            return _playerRegistry[key];
    }

    /// <summary>
    /// Registers a local tile.
    /// </summary>
    /// <param name="key"> Key for registering the tile. </param>
    /// <param name="value"> Local tile to store. </param>
    public void Register(Vector3 key, Tile value)
    {
        if (!_tileRegistry.ContainsKey(key))
            _tileRegistry.Add(key, value);
        else if (_tileRegistry[key] != value)
            throw new Exception($"Another local tile with the same ID {key} already registered!");
    }

    /// <summary>
    /// Deregisters a local tile.
    /// </summary>
    /// <param name="key"> Key for deregistering the tile. </param>
    public void Deregister(Vector3 key)
    {
        if (!_tileRegistry.Remove(key))
            throw new Exception($"No local tile with the ID {key} is registered!");
    }

    /// <summary>
    /// Finds and returns a tile if present.
    /// </summary>
    /// <param name="key"> Key to use for retrieval. </param>
    /// <returns> Registered local tile. </returns>
    public Tile Request(Vector3 key)
    {
        if (!_tileRegistry.ContainsKey(key))
            throw new Exception($"No local tile with the ID {key} is registered!");
        else
            return _tileRegistry[key];
    }
}
