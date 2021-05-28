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
            new Player(DPlayer.ID.Red),
            new Player(DPlayer.ID.Blue),
            new Player(DPlayer.ID.Yellow),
            new Player(DPlayer.ID.Green)
        };
    }
}
