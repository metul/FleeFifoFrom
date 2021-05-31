using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionTile : UiTile
{
    public DActionPosition.TileId Id;
    [SerializeField] private Button _tileButton;
    public bool Interactable { set { if(_tileButton != null) _tileButton.interactable = value; } }

    private void Start()
    {
        Interactable = false;
        _tileButton.onClick.AddListener(() => _buttonManager.OnActionTileClick(this));
    }
}