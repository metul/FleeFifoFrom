using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Meeple: MonoBehaviour
{
    public DMeeple Core { get; private set; }

    // public bool Interactable
    // {
    //     set
    //     {
    //         if(_tile != null)
    //             _tile.Interactable = value;
    //     }
    // }

    protected Action OnDefault;
    protected Action OnTapped;

    private Tile _tile;
    private FieldManager _fieldManager;
    private Transform _transform;
    private Renderer _renderer;
    
    public virtual void Initialize(DMeeple core, FieldManager fieldManager)
    {
        Core = core;
        _fieldManager = fieldManager;
        _transform = transform;
        
        SetTo(Core.Position.Current);
        core.Position.OnChange += SetTo;
        OnDefault += () => { SetColor(Color.gray); Debug.Log("Change Color debug!"); };
    }

    protected void SetColor(Color color)
    {
        var rend = GetComponent<Renderer>();
        rend.material.color = color;
    }

    public void Initialize(DMeeple core)
    {
        Initialize(core, FindObjectOfType<FieldManager>());
    }

    public void SetTo(Tile tile)
    {
        if (_tile == tile)
            return;

        if (_tile != null)
            _tile.Meeples.Remove(this);
        _tile = tile;
        _tile.Meeples.Add(this);
        _transform.SetParent(_tile.Transform);
        if (_tile.Meeples.Count > 1)
        {
            float angle = (float) _tile.Meeples.Count;
            _transform.localPosition = new Vector3
                (((float) Math.Cos(angle)) * .6f,
                0,
                -((float) Math.Sin(angle)) * .6f
            );
        }
        else
        {
            _transform.localPosition = Vector3.zero;
        }
    }

    protected void SetTo(DPosition position)
    {
        SetTo(_fieldManager.TileByPosition(position));
    }
}
