using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    // TODO replace with state manager
    private FieldManager _fieldManager;

    private void Awake()
    {
        _fieldManager = FindObjectOfType<FieldManager>();
    }

    public void Authorize()
    {
        Debug.Log("Player chose the authorize action");
        _fieldManager.CurrentGameState = FieldManager.DebugGameState.Authorize;
    }

    public void Swap()
    {
        Debug.Log("Player chose the swap action");
        _fieldManager.CurrentGameState = FieldManager.DebugGameState.Swap;
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
        _fieldManager.CurrentGameState = FieldManager.DebugGameState.Retreat;
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
}