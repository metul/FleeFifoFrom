using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HonorMarker : MonoBehaviour
{
    [SerializeField] private Image _handleImage;
    [SerializeField] private Slider _slider;

    public int Value { set => _slider.value = value; }

    public void Init()
    {
        
    }
}