using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Worker : MonoBehaviour
{
    // TODO should be some kind of enum from a GameManagr class
    // TODO set color based on player ID
    public int PlayerId { get; private set; }
    
    // passed to a higher management class for reference
    public int ID { get; set; }

    private void SetColor(int playerID)
    {
        PlayerId = playerID;
        
        // TODO get player colors from some global settings
        var player1Color = Color.red;
        var player2Color = Color.blue;
        var player3Color = Color.yellow;
        var player4Color = Color.green;

        var image = GetComponent<Image>();
    }
}
