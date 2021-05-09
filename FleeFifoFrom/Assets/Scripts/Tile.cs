using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Vector2 ID { get; set; }
    public bool Interactable { get; set; }

    private FieldManager _fieldManager;

    [SerializeField] private Color _heightlightColor;
    private Color _defaultColor;
    
    private Action _onHighlight;
    private Action _onDefault;
    private Material _material;

    public Meeple Meeple { get; private set; }

    private Transform _transform;

    private void Awake()
    {
        _fieldManager = FindObjectOfType<FieldManager>();
        _transform = transform;
        
        _material = transform.GetChild(0).GetComponent<Renderer>().material;
        _defaultColor = _material.color;

        _onHighlight += () => ChangeColor(_heightlightColor);
        _onDefault += () => ChangeColor(_defaultColor);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(!Interactable)
            return;
        
        _fieldManager.OnTileClicked(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(Interactable)
            _onHighlight?.Invoke();
        else
            _onDefault?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _onDefault?.Invoke();
    }

    private void ChangeColor(Color color)
    {
        _material.color = color;
    }

    public void SetMeeple(Meeple meeple)
    {
        if (Meeple != null)
        {
            Debug.LogWarning($"There is already {Meeple} on tile {ID}");
        }
        else
        {
            Meeple = meeple;
            var meepleTransform = Meeple.transform;
            meepleTransform.parent = _transform;
            meepleTransform.localPosition = Vector3.zero;
        }
    }

    public Meeple RemoveMeeple()
    {
        var meeple = Meeple;
        Meeple.transform.parent = null;
        Meeple = null;
        return meeple;
    }
}
