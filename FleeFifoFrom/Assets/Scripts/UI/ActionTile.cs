using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionTile : MonoBehaviour
{
    public bool Interactable { get; set; }
    public List<Worker> Worker = new List<Worker>();
}
