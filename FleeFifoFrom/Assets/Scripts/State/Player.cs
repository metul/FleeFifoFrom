using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DPlayer
{
    public enum ID
    {
        Red, Blue, Yellow, Green,
    }
    
    public DPlayer(ID id, string name) {
        Id = id;
        Name = name;
        Honor = new DHonor();
        StoredMeeple = new List<DMeeple>();
        
        //TODO remove debug
        Honor.Score.OnChange += s =>
        {
            Debug.Log($"Player {id}, {name} has now {s} honor");
        };
    }
    public ID Id { get; protected set; }
    public string Name { get; protected set; }

    public DHonor Honor { get; protected set; }
    
    // TODO make this an observable
    public List<DMeeple> StoredMeeple { get; protected set; }
    
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

    public void SaveMeeple(DMeeple meeple)
    {
        StoredMeeple.Add(meeple);
    }

    public void UnsaveMeeple(DMeeple meeple)
    {
        StoredMeeple.Remove(meeple);
    }
}
