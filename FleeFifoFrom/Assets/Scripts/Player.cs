using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public DPlayer.ID ID { get; set; }
    
    // TODO replace with nicer colors
    public static Color RedColor = Color.red;
    public static Color BlueColor = Color.blue;
    public static Color YellowColor = Color.yellow;
    public static Color GreenColor = Color.green;

    // TODO Add function, unless implemented in instance of PLayer
    public void add() { }

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
