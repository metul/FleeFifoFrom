using UnityEngine;
using UnityEngine.UI;

public class HintDisplay : MonoBehaviour
{
    private Text _text;
    private Image _image;

    private void Awake()
    {
        _text = GetComponentInChildren<Text>();
        _image = GetComponentInChildren<Image>();
        StateManager.OnStateUpdate += state =>
        {
            _text.text = Hint(state);
            _image.color = ColorUtils.GetPlayerColor(GameState.Instance.TurnPlayer().Id);
            _text.color = _image.color.TextColor();
        };
    }

    private string Hint(StateManager.State state)
    {
        switch (state)
        {
            case StateManager.State.Default:
                if (GameState.Instance.TurnType == GameState.TurnTypes.ActionTurn)
                {
                    return GameState.Instance.TurnActionPossible 
                        ? "Choose an action from the menu on the left" 
                        : "There is nothing else you can do for now";
                }
                else
                    return "Clean up the field, select a piece to move it forward or chose a reset action on the right";
            case StateManager.State.Authorize:
                return "Choose on of the pieces in the front to bring them inside the castle";
            case StateManager.State.Swap1:
                return "Choose the first piece to swap";
            case StateManager.State.Swap2:
                return "Choose an adjacent piece";
            case StateManager.State.RiotChooseKnight:
                return "Choose a Knight to start a RIOT! Put pay attention, riots will give you a lot of disgrace";
            case StateManager.State.RiotChoosePath:
                return "Choose the next step of the knight and knock over the person on it! You can't run into other knights";
            case StateManager.State.RiotAuthorize:
                return "Finish the riot and enter the castle";
            case StateManager.State.Revive:
                return "Choose an injured piece to heal them and gain honor for your good deed";
            case StateManager.State.Reprioritize:
                return "Click the up or down arrow on the priority display to change a priority of a class of people";
            case StateManager.State.RetreatChooseTile:
                return "Choose a tile for the knight to retreat";
            case StateManager.State.RetreatChooseKnight:
                return "Choose a knight to abandon the front line and run to safety";
            case StateManager.State.Villager:
                return "Choose an empty tile for a person to move up";
            case StateManager.State.MoveMeeple:
                return "Choose an empty tile for a person to move up";
            case StateManager.State.PoachSelectWorker:
                return "Select an opponents worker to get it for yourself. Gain disgrace.";
            case StateManager.State.Recall:
                return "Return all worker on an action map tile to their owner";
            case StateManager.State.Cooperate:
                return "Select an opponents worker to return them to their owner. Gain honor.";
            case StateManager.State.PayForAction:
                return "Choose a Worker from your display to pay for your chosen action";
            case StateManager.State.GameOver:
                return "Game over! Click the red 'X' button in the right corner to exit the game";
            default:
                return "Well, well, what will you do?";
        }
    }
}
