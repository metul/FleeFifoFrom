using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTile : UiTile
{
    public DPlayer.ID Id;
    [SerializeField] public Text _nameDisplay;
    
    public void Initialize(DPlayer.ID playerId)
    {
        Id = playerId;
        _nameDisplay.text = GameState.Instance.PlayerById(Id)?.Name;
    }
}