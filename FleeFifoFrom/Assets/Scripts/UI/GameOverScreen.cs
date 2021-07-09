using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private Text _text;
    [SerializeField] private GameObject _visualAnchor;

    private void Start()
    {
        _visualAnchor.SetActive(false);
        
        StateManager.OnStateUpdate += state =>
        {
            if(state != StateManager.State.GameOver)
                return;
            
            _visualAnchor.SetActive(true);
            var winner = GameState.Instance.GetWinner();
            var verb = winner.Count == 1 ? "was" : "were";
            _text.text = $"{PlayerListToString(winner)} {verb} victorious!";
        };
    }

    private string PlayerListToString(List<DPlayer> players)
    {
        string result = players[0].Name;

        for (int i = 1; i < players.Count; i++)
        {
            result = $"{result}, {players[i].Name}";
        }
        
        return result;
    }
}