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
        core.Position.OnChange += position => { Debug.Log(position); SetTo(position); };
        
        
        // TODO replace tapped / default states with new mechanic (visually)
        var rend = GetComponent<Renderer>();
        OnDefault += () => { rend.material.color = Color.white; };
        OnTapped += () => { rend.material.color = Color.yellow; };
        
        Core.QueueState.OnChange += q => {
            if (q == DMeeple.MeepleQueueState.Tapped)
                OnTapped.Invoke();
            else if (Core.IsHealthy())
                OnDefault.Invoke();
        };
    }

    public void Initialize(DMeeple core)
    {
        Initialize(core, FindObjectOfType<FieldManager>());
    }

    public void SetTo(Tile tile)
    {
        if (_tile != null)
            _tile.Meeples.Remove(this);
        _tile = tile;
        _tile.Meeples.Add(this);
        _transform.SetParent(_tile.Transform);
        _transform.localPosition = Vector3.zero;
    }

    protected void SetTo(DPosition position)
    {
        SetTo(_fieldManager.TileByPosition(position));
    }
}
