using System;
using System.Collections;
using System.Collections.Generic;

public class DPlayer
{
    public enum ID
    {
        Red, Blue, Yellow, Green,
    }

    public static DPlayer[] CreateAnonymousPlayers(ID[] ids)
    {
        List<DPlayer> result = new List<DPlayer>();
        int i = 1;

        foreach(ID id in ids)
        {
            result.Add(new DPlayer(id, $"Player {i}"));
        }

        return result.ToArray();
    }

    public static DPlayer[] CreateAnonymousPlayers(ushort num = 4)
    {
        ID[] ids = new ID[num];
        for (ushort i = 0; i < num; i++)
        {
            ids[i] = Rules.DEFAULT_PLAYERS[i];
        }

        return CreateAnonymousPlayers(ids);
    }

    public ID Id { get; protected set; }
    public string Name { get; protected set; }

    public DHonor Honor { get; protected set; }

    public DPlayer(ID id, string name) {
        Id = id;
        Name = name;
        Honor = new DHonor();
    }
}
