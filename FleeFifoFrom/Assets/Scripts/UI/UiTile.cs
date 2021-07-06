using System.Collections.Generic;
using UnityEngine;

public abstract class UiTile : MonoBehaviour
{
    public List<Worker> Workers { get; set; }= new List<Worker>();

    public RectTransform Transform { get => _workerAnchor; }
    
    protected ButtonManager _buttonManager;
    [SerializeField] protected RectTransform _workerAnchor;

    protected void Awake()
    {
        _buttonManager = FindObjectOfType<ButtonManager>();
    }

    public void SetWorkerInteractable(bool interactable)
    {
        foreach (var worker in Workers)
            worker.Interactable = interactable;
    }

    public void SetOpponentWorkerInteractable(bool interactable, DPlayer player)
    {
        foreach (var worker in Workers)
        {
            worker.Interactable = interactable && (worker.Core.Owner != player.Id);
        }
    }
}