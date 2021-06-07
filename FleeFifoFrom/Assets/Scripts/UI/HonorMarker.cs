using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HonorMarker : MonoBehaviour
{
    [SerializeField] private Image _handleImage;
    [SerializeField] private Slider _slider;

    public void Init(DPlayer player)
    {
        _handleImage.color = Player.GetPlayerColor(player.Id);

        player.Honor.Index.OnChange = h =>
        {
            _slider.value = h;
        };
    }
}