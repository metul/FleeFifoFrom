using UnityEngine;
using UnityEngine.UI;

public class DebugStateDisplay : MonoBehaviour
{
    private Text _text;

    private void Awake()
    {
        _text = GetComponentInChildren<Text>();
        StateManager.OnStateUpdate += state =>
        {
            _text.text = state.ToString();
        };
    }
}