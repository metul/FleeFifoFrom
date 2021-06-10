using System;
using UnityEngine;

public class PriorityTileManager: MonoBehaviour
{
    public const string FARMER_TYPE = "farmer";
    public const string MERCHANT_TYPE = "merchant";
    public const string SCHOLAR_TYPE = "scholar";
    public const string KNIGHT_TYPE = "knight";

    [SerializeField] private Transform _farmerPrio;
    [SerializeField] private Transform _merchantPrio;
    [SerializeField] private Transform _scholarPrio;
    [SerializeField] private Transform _knightPrio;
    [SerializeField] private GameObject _display;
    
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

                    // TODO: update positions of tiles
                }
            }
            else
            {
                if (_display.activeSelf)
                    _display.SetActive(false);
            }
        };
    }

    public void IncreasePriority(string type)
    {
        ChangePriority(1, type);
    }

    public void DecreasePriority(string type)
    {
        ChangePriority(-1, type);
    }

    private void ChangePriority(int amount, string type)
    {
        switch (type)
        {
            case FARMER_TYPE:
                Debug.Log("Prio change");
                break;
            case MERCHANT_TYPE:
                Debug.Log("Prio change");
                break;
            case SCHOLAR_TYPE:
                Debug.Log("Prio change");
                break;
            case KNIGHT_TYPE:
                Debug.Log("Prio change");
                break;
            default:
                Debug.LogWarning("Invalid Priority Tile type string");
                break;
        }
    }
}