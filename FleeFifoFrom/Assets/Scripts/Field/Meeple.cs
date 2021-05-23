using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Meeple: MonoBehaviour
{
    public DMeeple Core { get; protected set; }

    protected Action OnDefault;
    protected Action OnTapped;

    private void Awake()
    {
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
    }
}
