using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Meeple
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Initialize(DMeeple core, FieldManager fieldManager)
    {
        base.Initialize(core, fieldManager);

        if (core.GetType() == typeof(DKnight))
        {
            SetColor(Player.GetPlayerColor(((DKnight) core).Owner));
        }
    }
}
