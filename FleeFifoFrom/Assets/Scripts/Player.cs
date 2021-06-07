using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player 
{

    public static Color RedColor = new Color(0.94f,0.28f,0.44f);
    public static Color BlueColor = new Color(0.07f,0.54f,0.7f);
    public static Color YellowColor = new Color(1f,0.82f,0.4f);
    public static Color GreenColor = new Color(0.02f,0.84f,0.63f);
    
    // TODO deprecate getting player color via player.cs this and remove this class
    public static Color GetPlayerColor(DPlayer.ID playerID)
    {
        switch (playerID)
        {
            case DPlayer.ID.Red:
                return RedColor;
            case DPlayer.ID.Blue:
                return BlueColor;
            case DPlayer.ID.Yellow:
                return YellowColor;
            case DPlayer.ID.Green:
                return GreenColor;
            default:
                return Color.white;
        }
    }

    public List<Card> Cards;
    public int CardCount { get => Cards.Count; }
}
