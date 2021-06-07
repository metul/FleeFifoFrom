using UnityEngine;
using UnityEngine.UI;

public class HonorMarker : MonoBehaviour
{
    [SerializeField] private Image _handleImage;
    [SerializeField] private Slider _slider;

    public void Init(DPlayer player)
    {
        _handleImage.color = ColorUtils.GetPlayerColor(player.Id);

        player.Honor.Index.OnChange += h =>
        {
            Debug.Log("CHANGE HONOR MARKER");
            _slider.value = h;
        };
    }
}