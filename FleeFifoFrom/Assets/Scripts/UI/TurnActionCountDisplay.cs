using UnityEngine;
using UnityEngine.UI;

public class TurnActionCountDisplay : MonoBehaviour
{
    private Text _text;
    private Image _image;

    private void Awake()
    {
        _text = GetComponentInChildren<Text>();
        _image = GetComponentInChildren<Image>();
        GameState.Instance.TurnActionCount.OnChange += i => UpdateText();
    }

    private void Start()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        var amount = (Rules.TURN_ACTION_LIMIT - GameState.Instance.TurnActionCount.Current).ToString();
        var type = GameState.Instance.TurnType == GameState.TurnTypes.ActionTurn ? "" : "reset ";
        _text.text = $"You have {amount} {type}actions left";
        _image.color = Player.GetPlayerColor(GameState.Instance.TurnPlayer().Id);
        _text.color = _image.color.TextColor();
    }
}
