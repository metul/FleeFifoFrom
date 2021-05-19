using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    // TODO replace with state manager
    private FieldManager _fieldManager;

    [SerializeField] private ActionTile _authorizeWorkerDisplay;
    [SerializeField] private ActionTile _swapWorkerDisplay;
    [SerializeField] private ActionTile _riotWorkerDisplay;
    [SerializeField] private ActionTile _reviveWokerDisplay;
    [SerializeField] private ActionTile _objectiveWorkerDisplay;

    [SerializeField] private ActionTile _playerWorkerDisplay;

    private ActionTile[] _actionTiles;

    // card & action tile states:
    // objective:   - draw card
    // countermand1 - draw card
    //              - (select card)
    // poach:       - select one opponent worker
    //              - select card
    // recall:      - select action tile
    // cooperate:   - select one opponent worker
    

    private void Awake()
    {
        _fieldManager = FindObjectOfType<FieldManager>();
    }

    private void Start()
    {
        _actionTiles = new[]
        {
            _authorizeWorkerDisplay,
            _swapWorkerDisplay,
            _riotWorkerDisplay,
            _reviveWokerDisplay,
            _objectiveWorkerDisplay
        };

        StateManager.OnUpdateInteractability += UpdateInteractability;
    }

    private void UpdateInteractability()
    {
        switch (StateManager.GameState)
        {
            case StateManager.State.CountermandDrawCard:
                EnableElements(false, false, false, false);
                break;
            case StateManager.State.CountermandSelectCard:
                EnableElements(false, false, false, true);
                break;
            case StateManager.State.PoachSelectWorker:
                EnableElements(false, true, false, false);
                break;
            case StateManager.State.PoachSelectCard:
                EnableElements(false, false, false, true);
                break;
            case StateManager.State.Recall:
                EnableElements(true, false, false, false);
                break;
            case StateManager.State.Cooperate:
                EnableElements(false, true, false, false);
                break;
            case StateManager.State.PayForAction:
                EnableElements(false, false, true, false);
                break;
            default:
                EnableElements(false, false, false, false);
                break;
        }
    }

    private void EnableElements(bool tiles, bool worker, bool playerWorker, bool cards)
    {
        foreach (var actionTile in _actionTiles)
        {
            actionTile.Interactable = tiles;
            actionTile.SetWorkerInteractable(worker);
        }
        _playerWorkerDisplay.SetWorkerInteractable(playerWorker);
        
        // TODO enable card selection
    }


    public void OnActionTileClick(ActionTile actionTile)
    {
        Debug.Log("Action Tile has been clicked");
        switch (StateManager.GameState)
        {
            case StateManager.State.Recall:
                var workers = actionTile.RemoveAllWorker();
                foreach (var worker in workers)
                {
                    Debug.Log($"TODO: return worker {worker} to player {worker.PlayerId}");
                }

                StateManager.GameState = StateManager.State.Default;
                break;
            default:
                Debug.LogWarning("This is not a state in which action tile interaction is allowed!");
            break;
        }
    }

    public void OnWorkerClick(Worker worker)
    {
        switch (StateManager.GameState)
        {
            case StateManager.State.Cooperate: 
                worker.Tile.RemoveWorker(worker);
                Debug.Log($"TODO: return worker {worker} to player {worker.PlayerId}");
                StateManager.GameState = StateManager.State.Default;
                break;
            case StateManager.State.PoachSelectWorker:
                worker.Tile.RemoveWorker(worker);
                Debug.Log($"TODO: poach worker {worker} from player {worker.PlayerId}");
                StateManager.GameState = StateManager.State.PoachSelectCard;
                break;
            case StateManager.State.PayForAction:
                worker.Tile.RemoveWorker(worker);
                Debug.Log($"TODO: pay worker {worker} for a previous/next (?) action");
                StateManager.GameState = StateManager.State.PoachSelectCard;
                break;
            default:
                Debug.LogWarning("This is not a state in which worker interaction is allowed!");
                break;
        }
    }

    #region ui button onClicks

    public void Authorize()
    {
        Debug.Log("Player chose the authorize action");
        StateManager.GameState = StateManager.State.Authorize;
    }

    public void Swap()
    {
        Debug.Log("Player chose the swap action");
        StateManager.GameState = StateManager.State.Swap1;
    }

    public void Riot()
    {
        Debug.Log("Player chose the riot action");
        StateManager.GameState = StateManager.State.Riot;
    }

    public void Revive()
    {
        Debug.Log("Player chose the revive action");
        StateManager.GameState = StateManager.State.Revive;
    }

    public void Objective()
    {
        Debug.Log("Player chose the objective action");
        // TODO
    }

    public void Countermand()
    {
        Debug.Log("Player chose the countermand action");
        StateManager.GameState = StateManager.State.CountermandDrawCard;
    }

    public void Reprioritize()
    {
        Debug.Log("Player chose the reprioritize action");
        StateManager.GameState = StateManager.State.Reprioritize;
    }

    public void Retreat()
    {
        Debug.Log("Player chose the retreat action");
        StateManager.GameState = StateManager.State.RetreatChooseTile;
    }

    public void Recall()
    {
        Debug.Log("Player chose the recall action");
        StateManager.GameState = StateManager.State.Recall;
    }

    public void Cooperate()
    {
        Debug.Log("Player chose the cooperate action");
        StateManager.GameState = StateManager.State.Cooperate;
    }

    public void Poach()
    {
        Debug.Log("Player chose the poach action");
        StateManager.GameState = StateManager.State.PoachSelectCard;
    }

    public void Villager()
    {
        Debug.Log("Player chose the draw villager action");
        StateManager.GameState = StateManager.State.Villager;
    }

    public void Undo()
    {
        Debug.Log("Player chose the undo action");
        CommandProcessor.Instance.Undo();
    }

    public void EndTurn()
    {
        Debug.Log("End Player Turn");
        // TODO remove debug
        StateManager.GameState = StateManager.State.PayForAction;
    }
    
    #endregion
}