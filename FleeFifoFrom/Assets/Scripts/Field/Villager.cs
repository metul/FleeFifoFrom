using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : Meeple
{

    protected Action OnInjured;
    // Start is called before the first frame update

    private void Awake()
    {
        OnInjured += () => { SetColor(Color.red); };
    }

    public override void Initialize(DMeeple core, FieldManager fieldManager)
    {
        base.Initialize(core, fieldManager);

        if (core.GetType().IsSubclassOf(typeof(DVillager)))
        {
            ((DVillager) core).Health.OnChange += h => {
                if (h == DVillager.HealthStates.Injured)
                    OnInjured?.Invoke();
                else
                    OnDefault?.Invoke();
            };
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
