using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Field : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Color _heightlightColor;
    private Color _defaultColor;
    
    private Action _onHighlight;
    private Action _onDefault;
    private Material _material;

    private void Awake()
    {
        _material = transform.GetChild(0).GetComponent<Renderer>().material;
        _defaultColor = _material.color;

        _onHighlight += () => ChangeColor(_heightlightColor);
        _onDefault += () => ChangeColor(_defaultColor);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Field: OnPointerDown()");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _onHighlight?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _onDefault?.Invoke();
    }

    private void ChangeColor(Color color)
    {
        _material.color = color;
    }
}
