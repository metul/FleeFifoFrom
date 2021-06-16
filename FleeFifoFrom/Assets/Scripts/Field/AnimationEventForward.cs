using UnityEngine;

public class AnimationEventForward: MonoBehaviour
{
    [SerializeField] private Villager _villager;
    [SerializeField] private Knight _knight;

    public void SetInjuredColor()
    {
        _villager?.SetInjuredColor();
    }    
    
    public void SetUnInjuredColor()
    {
        _villager?.SetUnInjuredColor();
    }
}
