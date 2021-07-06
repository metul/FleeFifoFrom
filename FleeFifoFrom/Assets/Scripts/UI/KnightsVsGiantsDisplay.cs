using UnityEngine;
using UnityEngine.UI;

public class KnightsVsGiantsDisplay : MonoBehaviour
{
    [SerializeField] private Text _knightCounter;
    [SerializeField] private Text _giantCounter;

    private void Start()
    {
        _giantCounter.text = GameState.Instance.GiantStrength.ToString();
        _knightCounter.text = GameState.Instance.KnightsFightingCount.Current.ToString();
        GameState.Instance.KnightsFightingCount.OnChange = i =>
        {
            _knightCounter.text = i.ToString();
        };
        
        
    }
}
