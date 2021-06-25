using System;
using System.Collections.Generic;
using UnityEngine;

public class PriorityTileManager : MonoBehaviour
{
    private const string FARMER_TYPE = "farmer";
    private const string MERCHANT_TYPE = "merchant";
    private const string SCHOLAR_TYPE = "scholar";
    private const string KNIGHT_TYPE = "knight";

    public static readonly Dictionary<string, Type> STRING_TYPE_MAPPING = new Dictionary<string, Type>()
    {
        {FARMER_TYPE, typeof(DFarmer)},
        {MERCHANT_TYPE, typeof(DMerchant)},
        {SCHOLAR_TYPE, typeof(DScholar)},
        {KNIGHT_TYPE, typeof(DKnight)},
    };

    [SerializeField] private Transform _farmerPrio;
    [SerializeField] private Transform _merchantPrio;
    [SerializeField] private Transform _scholarPrio;
    [SerializeField] private Transform _knightPrio;

    [SerializeField] private GameObject _display;

    [SerializeField] private Transform _highPrio;
    [SerializeField] private Transform _mediumPrio;
    [SerializeField] private Transform _lowPrio;


    private void Start()
    {
        _display.SetActive(false);

        StateManager.OnStateUpdate += state =>
        {
            if (state == StateManager.State.Reprioritize)
            {
                if (!_display.activeSelf)
                {
                    _display.SetActive(true);
                }
            }
            else
            {
                if (_display.activeSelf)
                    _display.SetActive(false);
            }
        };

        GameState.Instance.Priorities[typeof(DFarmer)].Value.OnChange += prio =>
        {
            _farmerPrio.parent = TransformFromPrio(prio);
        };
        GameState.Instance.Priorities[typeof(DScholar)].Value.OnChange += prio =>
        {
            _scholarPrio.parent = TransformFromPrio(prio);
        };
        GameState.Instance.Priorities[typeof(DMerchant)].Value.OnChange += prio =>
        {
            _merchantPrio.parent = TransformFromPrio(prio);
        };
        GameState.Instance.Priorities[typeof(DKnight)].Value.OnChange += prio =>
        {
            _knightPrio.parent = TransformFromPrio(prio);
        };
    }

    public void IncreasePriority(string type)
    {
        ChangePriority(true, type);
    }

    public void DecreasePriority(string type)
    {
        ChangePriority(false, type);
    }

    private void ChangePriority(bool increase, string type)
    {
        CommandProcessor.Instance.ExecuteCommand(new ReprioritizeCommand(
            0, type, increase
        ));
        StateManager.CurrentState = StateManager.State.Default;
    }

    private Transform TransformFromPrio(DPrio.PrioValue prio)
    {
        switch (prio)
        {
            case DPrio.PrioValue.High:
                return _highPrio;
            case DPrio.PrioValue.Medium:
                return _mediumPrio;
            case DPrio.PrioValue.Low:
                return _lowPrio;
            default:
                return null;
        }
    }
}