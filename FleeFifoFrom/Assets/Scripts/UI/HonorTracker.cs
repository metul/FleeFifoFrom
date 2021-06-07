using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HonorTracker : MonoBehaviour
{
    [SerializeField] private HonorMarker _markerPrefab;
    [SerializeField] private Transform _markerParent;

    private void Start()
    {
        foreach (var dPlayer in GameState.Instance.Players)
        {
            var marker = Instantiate(_markerPrefab, _markerParent);
            marker.Init(dPlayer);
        }
    }
}
