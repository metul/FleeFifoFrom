using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    // TODO replace with state manager
    private FieldManager _fieldManager;

    [SerializeField] private Transform authorizeWorkerDisplay;
    [SerializeField] private Transform swapWorkerDisplay;
    [SerializeField] private Transform riotWorkerDisplay;
    
    
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

    public void OnActionTileClick(ActionTile actionTile)
    {
        Debug.Log("Action Tile has been clicked");
    }

    public void OnWorkerClick(Worker worker)
    {
        Debug.Log("Worker has been clicked");
    }

    public void Authorize()
    {
        Debug.Log("Player chose the authorize action");
        _fieldManager.CurrentGameState = FieldManager.DebugGameState.Authorize;
        CommunicationManager.Instance.PublishAction(_fieldManager.CurrentGameState);
    }

    public void Swap()
    {
        Debug.Log("Player chose the swap action");
        _fieldManager.CurrentGameState = FieldManager.DebugGameState.Swap1;
        CommunicationManager.Instance.PublishAction(_fieldManager.CurrentGameState);
    }

    public void Riot()
    {
        Debug.Log("Player chose the riot action");
        _fieldManager.CurrentGameState = FieldManager.DebugGameState.Riot;
        CommunicationManager.Instance.PublishAction(_fieldManager.CurrentGameState);
    }

    public void Revive()
    {
        Debug.Log("Player chose the revive action");
        _fieldManager.CurrentGameState = FieldManager.DebugGameState.Revive;
        CommunicationManager.Instance.PublishAction(_fieldManager.CurrentGameState);
    }

    public void Objective()
    {
        Debug.Log("Player chose the objective action");
    }

    public void Countermand()
    {
        Debug.Log("Player chose the countermand action");
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
    }

    public void Cooperate()
    {
        Debug.Log("Player chose the cooperate action");
    }

    public void Poach()
    {
        Debug.Log("Player chose the poach action");
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
}