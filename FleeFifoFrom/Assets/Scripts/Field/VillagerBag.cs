using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerBag: MonoBehaviour
{
    //TODO: Either this is an abstract class method and the Reset command uses an instance of villager bag
    //Or there is a global bag instance

    int totalPieces =0; //placeholder
    int elders;
    int children;
    int commoners;
  
    private void Awake()
    {
    }

    //TODO: Define starting bag
    public void OnServerInitialized()
    {
        //TODO: Define bag as collection with preset probability
        //      of each piece
        //TODO: Probably makes sense to store these piece counts in a separate class
        // for easy modification?
        this.elders = 6;
        this.children = 8;
        this.commoners = 12;

        //TODO: Might be nicer to link the terminal condition to a count of total tiles
        for (int i =0; i<15; i++) 
        {
            DrawVillager();
            //TODO: Place villager somewhere
        }
    }

    //TODO: Either this is a static method, or the Reset command uses an instance of villager bag
    // I think it makes sense to have a global VillgerBag object
    public static void DrawVillager()
    {
        //TODO: Draw random object from collection
    }

}
