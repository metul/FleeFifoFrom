using UnityEngine;
using UnityEngine.UI;

public class TurnTypeDisplay: MonoBehaviour
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
        var type = GameState.Instance.TurnType == GameState.TurnTypes.ActionTurn ? "Action" : "Reset ";
        _text.text = $"{type} Turn";
        _image.color = ColorUtils.GetPlayerColor(GameState.Instance.TurnPlayer().Id);
        _text.color = _image.color.TextColor();
    }
}