using UnityEngine;
using UnityEngine.UI;

public class Worker : MonoBehaviour
{
    public DWorker Core { get; private set; }
    public bool Interactable
    {
        get => _button.interactable;
        set
        {
            if(_button != null)
                _button.interactable = value;   
        }
    }
    private UiTile _tile;
    private Button _button;
    private ButtonManager _buttonManager;
    private Transform _transform;

    public void Initialize(DWorker core, ButtonManager buttonManager)
    {
        Core = core;
        _buttonManager = buttonManager;
        _button = GetComponent<Button>();
        _transform = transform;
        
        Interactable = false;
        SetColor(Core.Owner);
        SetTo(Core.Position.Current);

        Core.Position.OnChange += position => { SetTo(position); };
        _button.onClick.AddListener(() => _buttonManager.OnWorkerClick(this));

    }

    private void SetTo(UiTile tile)
    {
        if(_tile != null)
            _tile.Workers.Remove(this);
        _tile = tile;
        _tile.Workers.Add(this);
        _transform.SetParent(_tile.Transform);
    }

    private void SetTo(DActionPosition position)
    {
        SetTo(_buttonManager.ActionTileByPosition(position));
    }

    private void SetColor(DPlayer.ID playerID)
    {
        var image = GetComponent<Image>();
        image.color = Player.GetPlayerColor(playerID);
    }
}
