using System;
using System.Collections;
using UnityEngine;

public class Villager : Meeple
{
    private static readonly int ANIM_INJURED = Animator.StringToHash("injuredTrigger");
    private static readonly int ANIM_UNINJURED = Animator.StringToHash("unInjuredTrigger");
    
    [SerializeField] private Color _injuredColor = Color.red;
    
    protected Action OnInjured;
    protected Action OnUnInjured;


    public override void Initialize(DMeeple core, FieldManager fieldManager)
    {
        base.Initialize(core, fieldManager);

        if (core.GetType().IsSubclassOf(typeof(DVillager)))
        {
            ((DVillager) core).Health.OnChange += h => {
                if (h == DVillager.HealthStates.Injured)
                    OnInjured?.Invoke();
                else
                    OnUnInjured?.Invoke();
            };
        }
    }

    protected override void Start()
    {
        base.Start();
        OnInjured += () =>
        {
            _animator.SetTrigger(ANIM_INJURED);
        };
        OnUnInjured += () =>
        {
            _animator.SetTrigger(ANIM_UNINJURED);
        };
    }

    // called by animatior
    public void SetInjuredColor()
    {
        SetColor(_injuredColor);
    }    
    
    public void SetUnInjuredColor()
    {
        SetColor(_defaultColor);
    }
}
