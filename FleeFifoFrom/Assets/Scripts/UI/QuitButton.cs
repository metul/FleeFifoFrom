using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    
    private void Start()
    {
        _button.onClick.AddListener(() => { Application.Quit();});
    }
}
