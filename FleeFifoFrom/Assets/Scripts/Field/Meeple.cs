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

    private void Awake()
    {
        // get references
        _fieldManager = FindObjectOfType<FieldManager>();
        
        // Debug
        var rend = GetComponent<Renderer>();
        OnDefault += () => { rend.material.color = Color.white; };
        OnTapped += () => { rend.material.color = Color.yellow; };
    }

    public virtual void Initialize(DMeeple core)
    {
        Core = core;
        Core.QueueState.OnChange += q => {
            if (q == DMeeple.MeepleQueueState.Tapped)
                OnTapped.Invoke();
            else if (Core.IsHealthy())
                OnDefault.Invoke();
        };
        
        // change tile on position change
        core.Position.OnChange += p =>
        {
            // TODO: remove this (it currently replaces authorize)
            if (p == null)
            {
                // TODO: remove meeple from GameState
                Destroy(this.gameObject);
            }
            else
            {
                _fieldManager.TileByPosition(p).SetMeeple(this);
            }
        };
    }
}
