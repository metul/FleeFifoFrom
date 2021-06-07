using UnityEngine;

public class PlayerStatDisplaySpawner : MonoBehaviour
{
    [SerializeField] private PlayerStatsDisplay _playerStatsDisplayPrefab;

    private void Start()
    {
        var t = transform;
        foreach (var dPlayer in GameState.Instance.Players)
        {
            var display = Instantiate(_playerStatsDisplayPrefab, t);
            display.Init(dPlayer);
        }
    }
}