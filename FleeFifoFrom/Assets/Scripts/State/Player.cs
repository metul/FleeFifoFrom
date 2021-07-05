using System;
using System.Collections.Generic;
using UnityEngine;

public class DPlayer
{
    public enum ID
    {
        Black = -1, // TODO: Used only as a defult value currently, coloring etc. needs to be handled
        Red = 0, 
        Blue = 1, 
        Green = 3,
        Yellow = 2 
    }

    public DPlayer(ID id, string name)
    {
        Id = id;
        Name = name;
        Honor = new DHonor();
        RegistryManager.Instance.Register(Id, this);
    }
    public ID Id { get; protected set; }
    public string Name { get; protected set; }
    public DHonor Honor { get; protected set; }

    public Action OnDeAuthorize;

    public static DPlayer[] CreateAnonymousPlayers(ID[] ids)
    {
        List<DPlayer> result = new List<DPlayer>();
        int i = 1;

        foreach (ID id in ids)
        {
            result.Add(new DPlayer(id, $"Player {i++}"));
        }

        return result.ToArray();
    }

    public static DPlayer[] CreateAnonymousPlayers() // TODO: revert
    {
        ID[] ids = new ID[GameState.PlayerCount];
        for (ushort i = 0; i < GameState.PlayerCount; i++)
        {
            ids[i] = Rules.DEFAULT_PLAYERS[i];
        }
        return CreateAnonymousPlayers(ids);
    }

    public override bool Equals(object obj)
    {
        if ((obj == null) || !GetType().Equals(obj.GetType()))
            return false;
        else
        {
            DPlayer otherPlayer = (DPlayer)obj;
            return (Id == otherPlayer.Id) && Name.Equals(otherPlayer.Name);
        }
    }
}
