using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HonorTracker : MonoBehaviour
{
    [SerializeField] private Slider _markerPrefab;

    private void Start()
    {
        // create some player and values for debug
        var players = new[]
        {
            new Player(Player.PlayerID.Red),
            new Player(Player.PlayerID.Blue),
            new Player(Player.PlayerID.Yellow),
            new Player(Player.PlayerID.Green)
        };
    }
}
