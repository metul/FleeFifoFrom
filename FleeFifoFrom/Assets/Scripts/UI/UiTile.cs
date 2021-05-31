using System.Collections.Generic;
using UnityEngine;

public abstract class UiTile : MonoBehaviour
{
    public List<Worker> Workers { get; set; }= new List<Worker>();

    public Transform Transform { get => _workerAnchor; }
    
    protected ButtonManager _buttonManager;
    [SerializeField] protected Transform _workerAnchor;

    protected void Awake()
    {
        _buttonManager = FindObjectOfType<ButtonManager>();
    }

    public void SetWorkerInteractable(bool interactable)
    {
        foreach (var worker in Workers)
            worker.Interactable = interactable;
    }
}