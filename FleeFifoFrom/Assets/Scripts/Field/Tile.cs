using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    // TODO: deprecate this with Position (keep in mind Pos starts with 1 not 0)
    public Vector2 ID { get; set; }
    public DPosition Position
    {
        get => new DPosition((ushort) (ID.x + 1), (ushort) (ID.y + 1));
    }
    public bool Interactable { get; set; }
    public List<Meeple> Meeples { get; private set; } = new List<Meeple>();
    //public List<Villager> Villagers { get; private set; } = new List<Villager>();
    public Transform Transform { get; private set; }

    [SerializeField] private Color _heightlightColor;
    
    private FieldManager _fieldManager;
    private Action _onHighlight;
    private Action _onDefault;
    private Material _material;
    private Color _defaultColor;

    private void Awake()
    {
        Transform = transform;
        _fieldManager = FindObjectOfType<FieldManager>();
        _material = transform.GetChild(0).GetComponent<Renderer>().material;
        _defaultColor = _material.color;

        _onHighlight += () => ChangeColor(_heightlightColor);
        _onDefault += () => ChangeColor(_defaultColor);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(Interactable)
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
}
