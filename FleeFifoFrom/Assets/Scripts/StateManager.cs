using MLAPI;
using MLAPI.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public enum State
    {
        Default,
        Authorize,
        Swap1,
        Swap2,
        RiotChooseKnight,
        RiotChoosePath,
        RiotAuthorize,
        Revive,
        Reprioritize,
        RetreatChooseTile,
        RetreatChooseKnight,
        Villager,
        MoveMeeple,
        CountermandDrawCard,
        CountermandSelectCard,
        PoachSelectWorker,
        PoachSelectCard,
        Recall,
        Cooperate,
        PayForAction,
        GameOver
    }

    private static readonly State[] UNDO_MILESTONES =
    {
        State.Default, State.RiotChoosePath, State.RiotAuthorize
    };

    private static State _currentState;
    private static Stack<State> _stateStack = new Stack<State>();
    
    public static Action<State> OnStateUpdate;

    public static State CurrentlyPayingFor;

    public static State CurrentState
    {
        get => _currentState;
        set
        {
            State prevState = _currentState; // MARK: Used for network logging
            if (value == State.PayForAction)
            {
                CurrentlyPayingFor = _currentState;
            }

            _currentState = value;
            _stateStack.Push(value);
            OnStateUpdate?.Invoke(value);
            // MARK: Update network state if connected, supports local debugging
            if ((NetworkManager.Singleton?.IsConnectedClient).GetValueOrDefault())
                NetworkStateManager.Instance.NetworkCurrentState.Value = (int) value;
        }
    }

    private void Start()
    {
        CurrentState = State.Default;
        GameState.Instance.OnTurnChange += types => { CurrentState = State.Default; };

        GameState.Instance.OnTurnChange += _ =>
        {
            _stateStack.Clear();
            CurrentState = State.Default;
        };
    }

    public static void Undo()
    {
        if (_stateStack.Count <= 1)
            return;

        _stateStack.Pop();
        var previousState = _stateStack.Peek();
        _currentState = previousState;
        OnStateUpdate?.Invoke(previousState);
    }

    public static void UndoUntilLastMilestone()
    {
        var abort = false;
        var changedState = false;
        while (_stateStack.Count > 1 && !abort)
        {
            _stateStack.Pop();
            var previousState = _stateStack.Peek();
            _currentState = previousState;
            changedState = true;

            if (UNDO_MILESTONES.Contains(previousState))
                abort = true;
        }

        if (changedState)
            OnStateUpdate?.Invoke(_stateStack.Peek());
    }

    public static bool IsCurrentStateMilestone()
    {
        return UNDO_MILESTONES.Contains(_currentState);
    }

    public static bool IsRiotStep()
    {
        return CurrentState == State.RiotChoosePath || CurrentState == State.RiotAuthorize;
    }
}