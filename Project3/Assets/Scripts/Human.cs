using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : VehicleAbstract
{


    [SerializeField]
    float predictionTime = 1.0f;

    int OBCounter = 0;

    CollisionDetection detection;

    [SerializeField]
    GameObject toCreate;

    public bool alive;

    GameManager manager;

    [SerializeField]
    private float sightDistance = 6.5f;

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

        base.Start();

        // Flag saying if a human should be chased or not
        alive = true;

        detection = new CollisionDetection();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        if (!alive)
        {
            Object.Destroy(gameObject);
        }

        /*
        bool spawned = false;

        foreach (VehicleAbstract o in manager.zombieList) {
            Bounds mine = myBounds;
            Bounds theirs = o.myBounds;

            bool check = detection.CircleCollision(mine, theirs);

            // If there is a zombie colliding, then make one right here
            if (check && !spawned)
            {
                Debug.Log("The human and zombie are colliding");
                GameObject newZombie = Instantiate(toCreate, vehiclePosition, Quaternion.identity);
                spawned = true;

            }

        }

        if (spawned)
        {
            foreach(Zombie z in zombies)
            {
                // z.RemoveHuman(this);
            }


        }
        */
        
    }




    public override void CalcSteeringForces()
    {
        if (!alive)
        {
            // Imediately break out of this function, because the human doesn't need to do any calcs
            return;
        }

        ultimaForce = Vector3.zero;

        foreach(Zombie o in manager.zombieList)
        {
            if (!o.active)
            {
                // If the zombie is not active then simply ignore it
                continue;
            }

            if (Vector3.Distance(vehiclePosition, o.getVehiclePosition()) <= sightDistance)

                ultimaForce += Evade(o, predictionTime);
        }

        if(ultimaForce == Vector3.zero)
        {
            /*
            if (wanderCount > 0)
            {
                // Lower the count as we don't want to update things yet
                wanderCount--;
            }
            else
            {
                // Means that no zombies are close, so wander
                ultimaForce += Wander();
                wanderCount = wanderOriginalCount;
            }
            */
            ultimaForce += Wander();
            wanderCount = wanderOriginalCount;
        }

        // If I have multiple forces to apply to ultima, do I add them at the end, after all the forces got added, or
        // as I'm adding them? Aka do I do it in the above loop or not

        // ADD ALL THE VALUES TO ULTIMA FORCE, THEN NORMALIZE AND APPLY IT

        ultimaForce += Separate(manager.humansList);

        // Bounds Logic
        if (CheckIsPointOutBounds(getVehiclePosition()))
        {
            ultimaForce = SeekCenter(); //* ++OBCounter;
        }
        else
        {
            OBCounter = 0;
        }

        ultimaForce += AvoidAllObstacles(manager.obstaclesList);


        ultimaForce = ultimaForce.normalized * maxSteeringForce;

        ultimaForce.y = 0.0f;

        ApplyForce(ultimaForce);

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(vehiclePosition, sightDistance);
    }

}
