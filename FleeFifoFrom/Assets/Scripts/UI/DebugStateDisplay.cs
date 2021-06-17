using UnityEngine;
using UnityEngine.UI;

public class DebugStateDisplay : MonoBehaviour
{
    private Text _text;

    private void Awake()
    {
        _text = GetComponentInChildren<Text>();
        StateManager.Instance.OnStateUpdate += ModifyText;
    }

    // MARK: Temporarily set as public for debugging
    public void ModifyText(StateManager.State state)
    {
        _text.text = state.ToString();
    }
}
