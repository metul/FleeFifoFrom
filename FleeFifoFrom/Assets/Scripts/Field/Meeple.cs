using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Meeple: MonoBehaviour
{
    private FieldManager _fieldManager;
    public DMeeple Core { get; protected set; }

    protected Action OnDefault;
    protected Action OnTapped;
    
    // TODO decouple field manager from meeple -> set tile from meeple?
    public virtual void Initialize(DMeeple core, FieldManager fieldManager)
    {
        Core = core;
        _fieldManager = fieldManager;

        var rend = GetComponent<Renderer>();
        OnDefault += () => { rend.material.color = Color.white; };
        OnTapped += () => { rend.material.color = Color.yellow; };
        
        Core.QueueState.OnChange += q => {
            if (q == DMeeple.MeepleQueueState.Tapped)
                OnTapped.Invoke();
            else if (Core.IsHealthy())
                OnDefault.Invoke();
        };
        
        core.Position.OnChange += p =>
        {
            Debug.Log($"This is {this}");
            _fieldManager.TileByPosition(p).SetMeeple(this);
        };
    }
}
