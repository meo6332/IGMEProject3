using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fleer : VehicleAbstract
{

    [SerializeField]
    VehicleAbstract pursuer;

    // Start is called before the first frame update
    void Start()
    {

        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        

        base.Update();
    }


    public override void CalcSteeringForces()
    {
        if (pursuer == null)
        {
            var objects = FindObjectsOfType<GameObject>();

            // If the target doesn't exist, find it
            foreach (GameObject o in objects)
            {
                if (o.name.Contains("Seeker("))
                {
                    pursuer = o.GetComponent<VehicleAbstract>(); ;
                }
            }

        }

        else if (pursuer != null)
        {
            ultimaForce = Vector3.zero;
            ultimaForce += Flee(pursuer);

            ultimaForce = ultimaForce.normalized * maxSteeringForce;

            ApplyForce(ultimaForce);
        }
    }
}
