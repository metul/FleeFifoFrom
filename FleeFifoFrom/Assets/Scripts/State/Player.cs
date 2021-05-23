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

    private ushort _honorIndex;
    public short Honor
    {
        get => Rules.HONOR_VALUES[_honorIndex];
    }

    public DPlayer(ID id, string name) {
        Id = id;
        Name = name;
        _honorIndex = (ushort) Rules.HONOR_VALUES[Rules.HONOR_VALUES.Length / 2];
    }

    public void EarnHonor(ushort points = 1) {
        _honorIndex = (ushort) Math.Min(_honorIndex + points, Rules.HONOR_VALUES.Length - 1);
    }

    public void EarnDisgrace(ushort points = 1) {
        _honorIndex = (ushort) Math.Max(_honorIndex - points, 0);
    }
}
