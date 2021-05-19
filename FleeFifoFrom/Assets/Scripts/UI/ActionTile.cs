using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionTile : MonoBehaviour
{
    // TODO id
    public bool Interactable
    {
        get => _tileButton.interactable;
        set => _tileButton.interactable = value;
    }
    
    [SerializeField] private Button _tileButton;
    private ButtonManager _buttonManager;
    private Transform _transform;
    
    private List<Worker> _workers = new List<Worker>();

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _buttonManager = FindObjectOfType<ButtonManager>();
    }

    private void Start()
    {
        Interactable = false;
        _tileButton.onClick.AddListener(() => _buttonManager.OnActionTileClick(this));
        
        // TODO: remove this debug
        DebugInit();
    }

    private void DebugInit()
    {
        _workers = new List<Worker>(_transform.GetComponentsInChildren<Worker>());
        for (var i = 0; i < _workers.Count; i++)
        {
            var worker = _workers[i];
            worker.Tile = this;
        }
    }

    public void AddWorker(Worker worker)
    {
        worker.Tile = this;
        _workers.Add(worker);
        worker.transform.parent = _transform;
    }

    public Worker RemoveWorker(Worker worker)
    {
        _workers.Remove(worker);
        worker.transform.parent = null;
        return worker;
    }

    public List<Worker> RemoveAllWorker()
    {
        List<Worker> workers = new List<Worker>(_workers);
        
        foreach (var worker in _workers)
            worker.transform.parent = null;
        
        _workers.Clear();
        return workers;
    }

    public void SetWorkerInteractable(bool interactable)
    {
        foreach (var worker in _workers)
        {
            worker.Interactable = interactable;
        }
    }
}
