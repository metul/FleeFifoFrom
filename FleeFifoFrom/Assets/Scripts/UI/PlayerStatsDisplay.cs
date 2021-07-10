using System;
using UnityEngine;
using UnityEngine.UI;


public class PlayerStatsDisplay : MonoBehaviour
{
    [SerializeField] private Text _textHonor;
    [SerializeField] private Text _textSaved;
    [SerializeField] private Text _textTotal;

    [SerializeField] private Image _imageHonor;
    [SerializeField] private Image _imageSaved;
    [SerializeField] private Image _imageTotal;

    public void Init(DPlayer player)
    {
        var colorPlayer = ColorUtils.GetPlayerColor(player.Id);
        var colorText = colorPlayer.TextColor();

        _textHonor.color = colorPlayer;
        _textSaved.color = colorPlayer;
        _textTotal.color = colorPlayer;

        // _imageHonor.color = colorPlayer;
        // _imageSaved.color = colorPlayer;
        // _imageTotal.color = colorPlayer;

        player.Honor.Score.OnChange += h =>
        {
            _textHonor.text = h.ToString();
            _textTotal.text = GameState.Instance.PlayerScore(player.Id).ToString();
        };
        
        player.OnDeAuthorize += () =>
        {
            var honor = player.Honor.Score.Current;
            _textSaved.text = GameStateUtils.AuthorizedVillagers(GameState.Instance, player.Id).Length.ToString();
            _textTotal.text = GameState.Instance.PlayerScore(player.Id).ToString();
        };
    }
}