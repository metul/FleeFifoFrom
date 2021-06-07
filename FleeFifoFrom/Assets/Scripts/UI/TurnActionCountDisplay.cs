using UnityEngine;
using UnityEngine.UI;

public class TurnActionCountDisplay : MonoBehaviour
{
    private Text _text;

    private void Awake()
    {
        _text = GetComponentInChildren<Text>();
        GameState.Instance.TurnActionCount.OnChange += i => UpdateText();
    }

    private void Start()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        var amount = (Rules.TURN_ACTION_LIMIT - GameState.Instance.TurnActionCount.Current).ToString();
        _text.text = $"{amount} actions left";
    }
}
