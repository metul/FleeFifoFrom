using UnityEngine;
using UnityEngine.UI;

public class VillagerBagCount : MonoBehaviour
{
    [SerializeField] private Text _text;

    private void Start()
    {
        _text.text = GameState.Instance.VillagerBagCount.Current.ToString();
        GameState.Instance.VillagerBagCount.OnChange += i =>
        {
            _text.text = i.ToString();
        };
    }
}