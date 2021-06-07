using UnityEngine;
using UnityEngine.UI;

public class KnightsVsGiantsDisplay : MonoBehaviour
{
    [SerializeField] private Text _knightCounter;
    [SerializeField] private Text _giantCounter;

    void Start()
    {
        _giantCounter.text = Rules.GIANT_STRENGTH.ToString();
        _knightCounter.text = GameState.Instance.KnightsFightingCount.Current.ToString();
        GameState.Instance.KnightsFightingCount.OnChange = i =>
        {
            _knightCounter.text = i.ToString();
        };
        
        
    }
}
