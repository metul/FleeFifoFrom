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

    private ActionTile[] _actionTiles;

    // card & action tile states:
    // objective:   - draw card
    // countermand1 - draw card
    //              - (select card)
    // poach:       - select one opponent worker
    //              - select card
    // recall:      - select action tile
    // cooperate:   - select one opponent worker

    public enum DebugResetState
    {
        Default,
        CountermandDrawCard,
        CountermandSelectCard,
        PoachSelectWorker,
        PoachSelectCard,
        Recall,
        Cooperate
    }

    private DebugResetState _resetState;

    public DebugResetState CurrentResetState
    {
        get => _resetState;
        set
        {
            _resetState = value;
            UpdateInteractability();
        }
    }

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
    }

    private void UpdateInteractability()
    {
        switch (CurrentResetState)
        {
            case DebugResetState.Default:
                EnableActionTiles(false);
                EnableWorkers(false);
                EnableCardSelection(false);
                break;
            case DebugResetState.CountermandDrawCard:
                EnableActionTiles(false);
                EnableWorkers(false);
                EnableCardSelection(false);
                break;
            case DebugResetState.CountermandSelectCard:
                EnableActionTiles(false);
                EnableWorkers(false);
                EnableCardSelection(true);
                break;
            case DebugResetState.PoachSelectWorker:
                EnableActionTiles(false);
                EnableWorkers(true);
                EnableCardSelection(false);
                break;
            case DebugResetState.PoachSelectCard:
                EnableActionTiles(false);
                EnableWorkers(false);
                EnableCardSelection(true);
                break;
            case DebugResetState.Recall:
                EnableActionTiles(true);
                EnableWorkers(false);
                EnableCardSelection(false);
                break;
            case DebugResetState.Cooperate:
                EnableActionTiles(false);
                EnableWorkers(true);
                EnableCardSelection(false);
                break;
        }
    }

    private void EnableActionTiles(bool enabled)
    {
        foreach (var actionTile in _actionTiles)
        {
            actionTile.Interactable = enabled;
        }
    }

    private void EnableWorkers(bool enabled)
    {
        // TODO only enable opponent worker
        
        foreach (var actionTile in _actionTiles)
        {
            actionTile.SetWorkerInteractable(enabled);
        }
    }

    private void EnableCardSelection(bool enabled)
    {
        Debug.Log($"Enable Card Selection: {enabled}");
    }


    public void OnActionTileClick(ActionTile actionTile)
    {
        Debug.Log("Action Tile has been clicked");
        switch (CurrentResetState)
        {
            case DebugResetState.Recall:
                var workers = actionTile.RemoveAllWorker();
                foreach (var worker in workers)
                {
                    Debug.Log($"TODO: return worker {worker} to player {worker.PlayerId}");
                }

                CurrentResetState = DebugResetState.Default;
                break;
            default:
                Debug.LogWarning("This is not a state in which action tile interaction is allowed!");
            break;
        }
    }

    public void OnWorkerClick(Worker worker)
    {
        switch (CurrentResetState)
        {
            case DebugResetState.Cooperate:
                var w1 = worker.Tile.RemoveWorker(worker.ID);
                Debug.Log($"TODO: return worker {worker} to player {worker.PlayerId}");
                CurrentResetState = DebugResetState.Default;
                break;
            case DebugResetState.PoachSelectWorker:
                var w2 = worker.Tile.RemoveWorker(worker.ID);
                Debug.Log($"TODO: poach worker {worker} from player {worker.PlayerId}");
                CurrentResetState = DebugResetState.PoachSelectCard;
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
        _fieldManager.CurrentGameState = FieldManager.DebugGameState.Authorize;
    }

    public void Swap()
    {
        Debug.Log("Player chose the swap action");
        _fieldManager.CurrentGameState = FieldManager.DebugGameState.Swap1;
    }

    public void Riot()
    {
        Debug.Log("Player chose the riot action");
        _fieldManager.CurrentGameState = FieldManager.DebugGameState.Riot;
    }

    public void Revive()
    {
        Debug.Log("Player chose the revive action");
        _fieldManager.CurrentGameState = FieldManager.DebugGameState.Revive;
    }

    public void Objective()
    {
        Debug.Log("Player chose the objective action");
        // TODO
    }

    public void Countermand()
    {
        Debug.Log("Player chose the countermand action");
        CurrentResetState = DebugResetState.CountermandDrawCard;
    }

    public void Reprioritize()
    {
        Debug.Log("Player chose the reprioritize action");
        _fieldManager.CurrentGameState = FieldManager.DebugGameState.Reprioritize;
    }

    public void Retreat()
    {
        Debug.Log("Player chose the retreat action");
        _fieldManager.CurrentGameState = FieldManager.DebugGameState.RetreatChooseTile;
    }

    public void Recall()
    {
        Debug.Log("Player chose the recall action");
        CurrentResetState = DebugResetState.Recall;
    }

    public void Cooperate()
    {
        Debug.Log("Player chose the cooperate action");
        CurrentResetState = DebugResetState.Cooperate;
    }

    public void Poach()
    {
        Debug.Log("Player chose the poach action");
        CurrentResetState = DebugResetState.PoachSelectCard;
    }

    public void Villager()
    {
        Debug.Log("Player chose the draw villager action");
        _fieldManager.CurrentGameState = FieldManager.DebugGameState.Villager;
    }

    public void Undo()
    {
        Debug.Log("Player chose the undo action");
        CommandProcessor.Instance.Undo();
    }

    public void EndTurn()
    {
        Debug.Log("End Player Turn");
    }
    
    #endregion
}