using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    // TODO: deprecate this with Position.
    public Vector2 ID { get; set; }
    public bool Interactable { get; set; }
    public DPosition Position
    {
        get => new DPosition((ushort) (ID.x + 1), (ushort) (ID.y + 1));
    }

    private FieldManager _fieldManager;

    [SerializeField] private Color _heightlightColor;
    private Color _defaultColor;
    
    private Action _onHighlight;
    private Action _onDefault;
    private Material _material;

    public List<Meeple> Meeples { get; private set; }

    private Transform _transform;

    private void Awake()
    {
        Meeples = new List<Meeple>();
        
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
        
        _fieldManager.ProcessClickedTile(this);
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
        Meeples.Add(meeple);
        var meepleTransform = meeple.transform;
        meepleTransform.parent = _transform;
        meepleTransform.localPosition = Vector3.zero;
    }

    // removes the first meeple in the list
    public Meeple RemoveMeeple()
    {
        // empty check
        if (Meeples.Count == 0) return null;
        
        var meeple = Meeples[0];
        meeple.transform.parent = null;
        Meeples.RemoveAt(0);
        return meeple;
    }
}
