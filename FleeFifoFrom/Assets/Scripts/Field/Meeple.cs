using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Meeple: MonoBehaviour
{
    public enum State
    {
        Default, Tapped, Injured
    }
    
    private State _currentState;
    public State CurrentState
    {
        get => _currentState;
        set
        {
            ChangeState(value);
            _currentState = value;
        } 
    }
    
    private State _stateAfterMovement = State.Tapped;

    private Action OnDefault;
    private Action OnTapped;
    private Action OnInjured;

    private void Awake()
    {
        // Debug
        var rend = GetComponent<Renderer>();
        OnDefault += () => { rend.material.color = Color.white; };
        OnTapped += () => { rend.material.color = Color.yellow; };
        OnInjured += () => { rend.material.color = Color.red; };
        
        CurrentState = State.Default;
    }

    public void OnMove()
    {
        CurrentState = _stateAfterMovement;
    }
    
    private void ChangeState(State state)
    {
        switch (state)
        {
            case State.Default:
                OnDefault?.Invoke();
                break;
            case State.Tapped:
                OnTapped?.Invoke();
                break;
            case State.Injured:
                OnInjured?.Invoke();
                break;
        }
    }
}
