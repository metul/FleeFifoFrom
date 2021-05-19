using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionTile : MonoBehaviour
{
    public bool Interactable
    {
        set { if(_tileButton != null) _tileButton.interactable = value; }
    }

    [SerializeField] private Button _tileButton;
    private List<Worker> _workers = new List<Worker>();

    // references
    private ButtonManager _buttonManager;
    private Transform _transform;
    
    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _buttonManager = FindObjectOfType<ButtonManager>();
    }

    private void Start()
    {
        Interactable = false;
        if (_tileButton != null)
            _tileButton.onClick.AddListener(() => _buttonManager.OnActionTileClick(this));

        // TODO: remove this, debug
        Init(new List<Worker>(_transform.GetComponentsInChildren<Worker>()));
    }

    public void Init(List<Worker> workers)
    {
        _workers = workers;
        foreach (var worker in _workers)
            worker.Tile = this;
    }

    public void AddWorker(Worker worker)
    {
        worker.Tile = this;
        _workers.Add(worker);
        worker.transform.parent = _transform;
    }

    public void RemoveWorker(Worker worker)
    {
        _workers.Remove(worker);
        worker.transform.parent = null;
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