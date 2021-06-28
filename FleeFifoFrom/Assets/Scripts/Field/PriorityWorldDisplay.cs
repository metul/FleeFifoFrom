using System.Collections.Generic;
using UnityEngine;

public class PriorityWorldDisplay : MonoBehaviour
{
    private const int ARRAY_SIZE = 4;

    private class LocalPrioData
    {
        public Transform trans;
        public DPrio.PrioValue prio;
        public int index;
    }

    [SerializeField] private Transform _farmerPrio;
    [SerializeField] private Transform _merchantPrio;
    [SerializeField] private Transform _scholarPrio;
    [SerializeField] private Transform _knightPrio;

    private Dictionary<Transform, LocalPrioData> PRIO_DATA;
    private List<LocalPrioData> _data;

    [SerializeField] private Transform[] _highPrio = new Transform[ARRAY_SIZE];
    [SerializeField] private Transform[] _mediumPrio = new Transform[ARRAY_SIZE];
    [SerializeField] private Transform[] _lowPrio = new Transform[ARRAY_SIZE];

    private List<LocalPrioData> _highData = new List<LocalPrioData>();
    private List<LocalPrioData> _mediumData = new List<LocalPrioData>();
    private List<LocalPrioData> _lowData = new List<LocalPrioData>();
    

    // TODO: proper init with values prio values from rules and less hardcoding
    private void Start()
    {
        PRIO_DATA = new Dictionary<Transform, LocalPrioData>()
        {
            {
                _farmerPrio, new LocalPrioData
                {
                    trans = _farmerPrio,
                    prio = GameState.Instance.Priorities[typeof(DFarmer)].Value.Current,
                    index = 0
                }
            },
            {
                _scholarPrio, new LocalPrioData
                {
                    trans = _scholarPrio,
                    prio = GameState.Instance.Priorities[typeof(DScholar)].Value.Current,
                    index = 1
                }
            },
            {
                _merchantPrio, new LocalPrioData
                {
                    trans = _merchantPrio,
                    prio = GameState.Instance.Priorities[typeof(DMerchant)].Value.Current,
                    index = 0
                }
            },
            {
                _knightPrio, new LocalPrioData
                {
                    trans = _knightPrio,
                    prio = GameState.Instance.Priorities[typeof(DKnight)].Value.Current,
                    index = 0
                }
            },
        };
        
        foreach (var kvp in PRIO_DATA)
        {
            GetListFromPrio(kvp.Value.prio).Add(kvp.Value);
        }

        GameState.Instance.Priorities[typeof(DFarmer)].Value.OnChange += prio => { ChangePrio(prio, _farmerPrio); };
        GameState.Instance.Priorities[typeof(DScholar)].Value.OnChange += prio => { ChangePrio(prio, _scholarPrio); };
        GameState.Instance.Priorities[typeof(DMerchant)].Value.OnChange += prio => { ChangePrio(prio, _merchantPrio); };
        GameState.Instance.Priorities[typeof(DKnight)].Value.OnChange += prio => { ChangePrio(prio, _knightPrio); };
    }

    private Transform[] GetTransformFromPrio(DPrio.PrioValue prio)
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

    private List<LocalPrioData> GetListFromPrio(DPrio.PrioValue prio)
    {
        switch (prio)
        {
            case DPrio.PrioValue.High:
                return _highData;
            case DPrio.PrioValue.Medium:
                return _mediumData;
            case DPrio.PrioValue.Low:
                return _lowData;
            default:
                return null;
        }
    }

    private void ChangePrio(DPrio.PrioValue newPrio, Transform meeple)
    {
        var current = PRIO_DATA[meeple];
        var oldList = GetListFromPrio(current.prio);
        var othersNew = GetListFromPrio(newPrio);

        // remove from old list
        oldList.Remove(current);
        
        // move remaining abjects
        for (var i = current.index; i < oldList.Count; i++)
        {
            var moveThis = oldList[i];
            moveThis.index--;
            moveThis.trans.parent = GetTransformFromPrio(current.prio)[moveThis.index];
            moveThis.trans.localPosition = Vector3.zero;
        }

        // move current object to new position
        current.prio = newPrio;
        current.index = othersNew.Count;
        othersNew.Add(current);
        current.trans.parent = GetTransformFromPrio(current.prio)[current.index];
        current.trans.localPosition = Vector3.zero;
    }
}