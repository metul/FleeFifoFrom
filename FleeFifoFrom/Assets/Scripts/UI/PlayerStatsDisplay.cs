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

        _textHonor.color = colorText;
        _textSaved.color = colorText;
        _textTotal.color = colorText;

        _imageHonor.color = colorPlayer;
        _imageSaved.color = colorPlayer;
        _imageTotal.color = colorPlayer;

        // TODO calculate score in gamestate and not here
        // TODO figure out why Honor.Score dosn't work anymore
        player.Honor.Index.OnChange += h =>
        {
            var honor = Rules.HONOR_VALUES[h];
            _textHonor.text = honor.ToString();
            _textTotal.text = (GameState.Instance.PlayerScore(player.Id) + honor).ToString();
        };
        
        player.OnDeAuthorize += () =>
        {
            var honor = Rules.HONOR_VALUES[player.Honor.Index.Current];
            _textSaved.text = GameStateUtils.AuthorizedVillagers(GameState.Instance, player.Id).Length.ToString();
            _textTotal.text = (GameState.Instance.PlayerScore(player.Id) + honor).ToString();
        };
    }
}