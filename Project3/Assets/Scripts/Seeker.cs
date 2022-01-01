using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seeker : VehicleAbstract
{

    [SerializeField]
    VehicleAbstract target;

    // Start is called before the first frame update
    void Start()
    {

        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

        // Need to do this, or delete this class' update
        base.Update();
    }


    public override void CalcSteeringForces()
    {
        if (target == null)
        {
            var objects = FindObjectsOfType<GameObject>();

            // If the target doesn't exist, find it
            foreach(GameObject o in objects)
            {
                if (o.name.Contains("Fleer("))
                {
                    target = o.GetComponent<VehicleAbstract>(); ;
                }
            }

        }

        else if (target != null)
        {
            ultimaForce = Vector3.zero;
            ultimaForce += Seek(target);

            ultimaForce = ultimaForce.normalized * maxSteeringForce;

            ApplyForce(ultimaForce);
        }
    }

    /**
    public void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        // Will need to add the targets later
        foreach(VehicleAbstract current in targets)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, current.transform.position);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(current, current.CalcFuturePosition(futureTime);
        }

        // Make sure Zombie is seeking closest human
    }
    */

    // ADD FUTURE TIME AS THE OFFSET YOU NEED TO LOOK INTO THE FUTURE TO PREDICT TARGET'S POSTIION
}
