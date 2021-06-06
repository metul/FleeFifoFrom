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
            var _knight = (DKnight) core;
            switch (_knight.Owner)
            {
                case (DPlayer.ID.Red):
                    SetColor(Color.red);
                    break;
                case (DPlayer.ID.Blue):
                    SetColor(Color.blue);
                    break;
                case (DPlayer.ID.Green):
                    SetColor(Color.green);
                    break;
                case (DPlayer.ID.Yellow):
                    SetColor(Color.yellow);
                    break;
            }
        }
    }
}
