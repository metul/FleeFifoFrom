using UnityEngine;
using UnityEngine.UI;

public class HonorMarker : MonoBehaviour
{
    [SerializeField] private Image _handleImage;
    [SerializeField] private Slider _slider;
    [SerializeField] private int offset = 2;

    public void Init(DPlayer player)
    {
        _handleImage.color = ColorUtils.GetPlayerColor(player.Id);

        var cnt = (GameState.Instance.Players.Length + 1f) / 2f;
        var nr = ((int) player.Id + 1) - cnt;
        _slider.gameObject.GetComponent<RectTransform>().anchoredPosition += nr * offset * Vector2.right;  
        
        
        player.Honor.Index.OnChange += h =>
        {
            _slider.value = h;
        };
    }
}