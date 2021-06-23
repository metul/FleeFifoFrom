using System;
using System.Collections.Generic;
using UnityEngine;

public class DPlayer
{
    public enum ID
    {
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

    public static DPlayer[] CreateAnonymousPlayers(ushort num = 2)
    {
        ID[] ids = new ID[num];
        for (ushort i = 0; i < num; i++)
        {
            ids[i] = Rules.DEFAULT_PLAYERS[i];
        }
        return CreateAnonymousPlayers(ids);
    }
}
