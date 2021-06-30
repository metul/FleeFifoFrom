using UnityEngine;

public class CurrentPlayerColor : MonoBehaviour
{
    private Renderer _renderer;
    
    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        GameState.Instance.TurnActionCount.OnChange += i => UpdateColor();
        UpdateColor();
    }
    
    private void UpdateColor()
    {
        _renderer.material.color = ColorUtils.GetPlayerColor(GameState.Instance.TurnPlayer().Id);
    }
}
