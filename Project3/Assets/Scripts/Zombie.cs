using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : VehicleAbstract
{

    [SerializeField]
    float predictionTime = 0.5f;

    CollisionDetection detection;

    GameManager manager;

    public bool active;



    // Start is called before the first frame update
    void Start()
    {

        var objects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject o in objects)
        {
            if (o.name.Contains("GameManager"))
            {
                manager = o.GetComponent<GameManager>();
            }
        }

        //active = true;

        if (!active)
        {
            vehiclePosition = new Vector3(10.0f, -10.0f, 50.0f);
            transform.position = vehiclePosition;
        }

        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }


    public override void CalcSteeringForces()
    {
        if (!active)
        {
            ultimaForce = Vector3.zero;
            return;
        }

        ultimaForce = Vector3.zero;
        bool chasingFlag = false;

        foreach (Human o in manager.humansList)
        {
            var distance = Vector3.Distance(vehiclePosition, o.getVehiclePosition());
            float maxRange = 8.0f;


            if (distance >= 4.0f && distance <= maxRange)
            {
                ultimaForce += Pursue(o, predictionTime);
                chasingFlag = true;
            }

            if (distance < 4.0f)
            {
                //Debug.Log("Seeking Human");
                ultimaForce += Seek(o);

                chasingFlag = true;
            }
        }

        ultimaForce += Separate(manager.zombieList);

        if(manager.humansList.Count <= 0 || !chasingFlag)
        {
            // Means there are no humans, so wander
            ultimaForce += Wander();
        }

        // Bounds Logic
        if (CheckIsPointOutBounds(getVehiclePosition()))
        {
            ultimaForce = SeekCenter(); //* ++OBCounter;
        }

        ultimaForce += AvoidAllObstacles(manager.obstaclesList);

        ultimaForce = ultimaForce.normalized * maxSteeringForce;
        ultimaForce.y = 0.0f;

        ApplyForce(ultimaForce);
    }


    public void despawnSelf()
    {
        // TO COMPLETE LATER

        active = false;
        vehiclePosition = new Vector3(0.0f, -10.0f, 50.0f);
        transform.position = vehiclePosition;
    }

    public void spawnSelf(Vector3 newPos)
    {
        Debug.Log("Spawn Self called with a newPos of: " + newPos);

        active = true;
        vehiclePosition = newPos;
        transform.position = newPos;
    }

    /*
    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(transform.position, transform.position + velocity);

        // Add a DrawWireSphere for position with radius


        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(vehiclePosition, myBounds.extents.magnitude);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(vehiclePosition, 4.0f);

    }
    */


    }
