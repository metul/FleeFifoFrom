using UnityEngine;
using UnityEngine.UI;

public class Worker : MonoBehaviour
{
    public PlayerID PlayerId;
    public ActionTile Tile { get; set; }
    public bool Interactable
    {
        get => _button.interactable;
        set => _button.interactable = value;
    }

    private Button _button;
    private ButtonManager _buttonManager;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _buttonManager = FindObjectOfType<ButtonManager>();
    }

    private void Start()
    {
        SetColor(PlayerId);
        Interactable = false;
        _button.onClick.AddListener(() => _buttonManager.OnWorkerClick(this));
    }

    private void SetColor(PlayerID playerID)
    {
        var image = GetComponent<Image>();
        image.color = Player.GetPlayerColor(playerID);
    }
}
