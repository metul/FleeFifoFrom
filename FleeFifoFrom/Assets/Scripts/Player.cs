using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PlayerID
    {
        Red, Blue, Yellow, Green
    }
    public PlayerID ID { get; set; }
    
    // TODO replace with nicer colors
    public static Color RedColor = Color.red;
    public static Color BlueColor = Color.blue;
    public static Color YellowColor = Color.yellow;
    public static Color GreenColor = Color.green;

    // TODO Add function, unless implemented in instance of PLayer
    public void add() { }

    public static Color GetPlayerColor(PlayerID playerID)
    {
        switch (playerID)
        {
            case PlayerID.Red:
                return RedColor;
            case PlayerID.Blue:
                return BlueColor;
            case PlayerID.Yellow:
                return YellowColor;
            case PlayerID.Green:
                return GreenColor;
            default:
                return Color.white;
        }
    }

    public List<Card> Cards;
    public int CardCount { get => Cards.Count; }
}
