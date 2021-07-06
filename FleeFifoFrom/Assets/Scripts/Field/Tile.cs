using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 ID { get; set; } // (PosX - 1, PosY - 1, IsBattleFieldTile ? -1 : 1)
    public DPosition Position
    {
        get => new DPosition((ushort) (ID.x + 1), (ushort) (ID.y + 1));
    }
    public DPlayer.ID NominalOwner { get => _nominalOwner; }

    public bool Interactable
    {
        get => _interactable;
        set
        {
           if(_hovered && value)
               _onHighlight?.Invoke();
           _interactable = value;
        }
    }
    public List<Meeple> Meeples { get; private set; } = new List<Meeple>();
    public Transform Transform { get; private set; }

    [SerializeField] private Color _heightlightColor;
    [SerializeField] private DPlayer.ID _nominalOwner;

    // References
    private FieldManager _fieldManager;
    private AudioSource _audioSource;
    private Material _material;
    private Color _defaultColor;
    
    // Actions
    private Action _onHighlight;
    private Action _onDefault;
    private bool _hovered;
    private bool _interactable;

    private void Awake()
    {
        Transform = transform;
        _fieldManager = FindObjectOfType<FieldManager>();
        _material = transform.GetChild(0).GetComponent<Renderer>().material;
        _audioSource = GetComponent<AudioSource>();
        _defaultColor = _material.color;

        _onHighlight += () => ChangeColor(_heightlightColor);
        _onDefault += () => ChangeColor(_defaultColor);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Interactable)
        {
            _fieldManager.ProcessClickedTile(this);
            _audioSource.Play();
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(Interactable)
            _onHighlight?.Invoke();
        else
        {
            _hovered = true;
            _onDefault?.Invoke();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hovered = false;
        _onDefault?.Invoke();
    }

    private void ChangeColor(Color color)
    {
        _material.color = color;
    }

    public override bool Equals(object other)
    {
        if ((other == null) || !GetType().Equals(other.GetType()))
            return false;
        else
        {
            Tile otherTile = (Tile)other;
            return  (ID.z == otherTile.ID.z) && (ID.x == otherTile.ID.x) && (ID.y == otherTile.ID.y);
        }
    }
}
