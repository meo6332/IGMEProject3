using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class VehicleAbstract : MonoBehaviour
{
    // The variables above won't get used

    [SerializeField]
    protected float maxSpeed = 2f;

    [SerializeField]
    protected float maxSteeringForce = 3f;

    protected Vector3 ultimaForce = Vector3.zero;

    [SerializeField]
    public float mass;

    // The below values all relate to the movement of the vehicle
    [SerializeField]
    Vector3 direction = Vector3.right;

    [SerializeField]
    protected Vector3 vehiclePosition;

    [SerializeField]
    protected Vector3 velocity = Vector3.zero;

    [SerializeField]
    Vector3 acceleration = Vector3.zero;


    Transform floorTransform;

    [SerializeField]
    GameObject floor;


    // Will be a 3d square with a center and a size
    // This has a MAXIMUM AND A MINIMUM VECTOR, which contain
    // the max is top right, min vector is bottom left 
    Bounds floorBounds;

    // Things needed for obstacle avoidance
    [SerializeField]
    protected float myRadius = 1.0f;

    private Vector3 forward = Vector3.forward;
    private Vector3 right = Vector3.right;
    [SerializeField]
    public float obstacleViewDistance = 3f;


    // Things needed for wandering
    [SerializeField]
    private float wanderDistance;

    [SerializeField]
    private float wanderRadius;

    protected int wanderCount;
    protected int wanderOriginalCount;

    private float wanderAngle;

    // Things needed for seperate
    [SerializeField]
    protected float seperateDistance;

    // Start is called before the first frame update
    protected void Start()
    {
        vehiclePosition = transform.position;

        direction = Vector3.right;

        // Find the floor as it's going to be needed for bounds checking
        var objects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach(GameObject o in objects)
        {
            if (o.name.Contains("floor"))
            {
                floor = o;
                Transform worldTransform = o.transform;
                floorBounds = new Bounds(worldTransform.position, 10 * worldTransform.lossyScale);

                Debug.Log(floorBounds.size);
            }

        }
        // Build the bounds in a different way
        // Take the position of the floor object, get it's scale from the transform
        // (do transform.lossyScale.x to get the object's scale 
        // Look into using transform.lossyScale


        if(mass == 0)
        {
            // Prevents crashes from divide by zero errors
            mass = 1.0f;
        }

        floorTransform = floor.transform;

        // Check about this and see if it's working
        //floorBounds = new Bounds(floorTransform.position, floorTransform.lossyScale);

        wanderDistance = 4.0f;
        wanderRadius = 3.0f;
        wanderOriginalCount = 30;
        wanderCount = 0;
        wanderAngle = Random.Range(0.0f, 360.0f);
    }

    // Update is called once per frame
    protected void Update()
    {
        CalcSteeringForces();
        
        velocity = velocity + (acceleration * Time.deltaTime);

        vehiclePosition = vehiclePosition + (velocity * Time.deltaTime);

        if (velocity != Vector3.zero)
        {
            direction = velocity.normalized;

            forward = direction;
            right = Vector3.Cross(forward, Vector3.up);
        }

        transform.position = vehiclePosition;

        transform.rotation = Quaternion.LookRotation(velocity);


        acceleration = Vector3.zero;
    }


    public void SetUpPosition(Vector3 startPos)
    {
        vehiclePosition = startPos;
        direction = Vector3.right;
        velocity = Vector3.zero;
    }


    public void SetNewPosition(Vector3 newPos)
    {
        vehiclePosition = newPos;
    }

    public void DestroySelf()
    {
        Object.Destroy(gameObject);
    }


    // Child implements this 
    public abstract void CalcSteeringForces();


    protected void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }


    public Vector3 Seek(Vector3 targetPos)
    {
        // Steering forces are intended to get an object to face towards an object
        Vector3 desiredVelocity = targetPos - vehiclePosition;

        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        return desiredVelocity - velocity;
    }

    public Vector3 Seek(VehicleAbstract targetVehicle)
    {
        // Exists to make it easier to Seek a vehicle without grabbing it's
        // position directly
        return Seek(targetVehicle.transform.position);
    }


    public Vector3 Flee(Vector3 targetPos)
    {
        // I know it's the opposite of seek, so is it -1 times desiredVelocity-velocity?
        Vector3 desiredVelocity = vehiclePosition - targetPos;

        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        Vector3 fleeVelocity = (desiredVelocity - velocity);

        return fleeVelocity;

    }


    public Vector3 Flee(VehicleAbstract pursuer)
    {
        return Flee(pursuer.transform.position);
    }

    
    // These next few functions are for the project 3
    public Vector3 Pursue(Vector3 targetPos)
    {
        return Seek(targetPos);
    }

    public Vector3 Pursue(VehicleAbstract target, float time)
    {
        Vector3 futurePos = target.CalcFuturePosition(time);
        return Pursue(futurePos);
    }

    public Vector3 Evade(Vector3 pursuerPos)
    {
        return Flee(pursuerPos);
    }

    public Vector3 Evade(VehicleAbstract pursuer, float time)
    {
        Vector3 futurePos = pursuer.CalcFuturePosition(time);
        return Evade(futurePos);
    }

    public Vector3 Wander()
    {
        // Need to calculate the center of the circle ahead
        var center = velocity.normalized;
        center *= wanderDistance;

        var angle = Random.Range(wanderAngle - 45.0f, wanderAngle + 45.0f);
        wanderAngle = angle;
        // Convert the random angle to radians
        angle = angle * Mathf.Deg2Rad;

        // Now find the x and y positions on the circle 
        var xPos = center.x + (Mathf.Cos(angle) * wanderRadius);
        var zPos = center.z + (Mathf.Sin(angle) * wanderRadius);

        Vector3 wanderLocation = new Vector3(xPos, 0.0f, zPos);

        Vector3 finalPos = vehiclePosition + center + wanderLocation;

        return Seek(finalPos);
    }

    protected Vector3 Separate<T>(List<T> vehicles) where T : VehicleAbstract
    {
        Vector3 separationForce = Vector3.zero;

        // Go through the vehicles list
        foreach(VehicleAbstract v in vehicles)
        {
            float distance = CalcSquareDistance(v);

            if(distance < Mathf.Epsilon)
            {
                continue;
            }

            if(distance < 0.0001f)
            {
                distance = 0.0001f;
            }

            float personalSpaceRadius = seperateDistance * seperateDistance;

            if(distance < personalSpaceRadius)
            {
                separationForce += Flee(v) * (personalSpaceRadius / distance);
            }

        }

        return separationForce;
    }

    // Need this to exist to work with the zombies
    protected Vector3 Separate<T>(Queue<T> vehicles) where T : VehicleAbstract
    {
        Vector3 separationForce = Vector3.zero;

        // Go through the vehicles list
        foreach (VehicleAbstract v in vehicles)
        {
            float distance = CalcSquareDistance(v);

            if (distance < Mathf.Epsilon)
            {
                continue;
            }

            if (distance < 0.0001f)
            {
                distance = 0.0001f;
            }

            float personalSpaceRadius = seperateDistance * seperateDistance;

            if (distance < personalSpaceRadius)
            {
                separationForce += Flee(v) * (personalSpaceRadius / distance);
            }

        }

        return separationForce;
    }


    protected float CalcSquareDistance(VehicleAbstract other)
    {
        return Vector3.SqrMagnitude(other.getVehiclePosition() - vehiclePosition);
    }

    public Vector3 CalcFuturePosition(float time)
    {
        // Position + (velocity * time)
        return (vehiclePosition + (velocity * time));

    }

    public Vector3 SeekCenter()
    {
        // Will seek the center point
        // Serves as a shortcut basically
        return Seek(floorBounds.center);
    }

    protected bool CheckIsPointOutBounds(Vector3 point)
    {
        // fill in the logic here
        float baseOffset = 0.0f;
        float offset = 4.0f;
        float halfWidth = floorBounds.size.x / 2;
        float halfHeight = floorBounds.size.z / 2;

        // Debug.Log("point position: " + point);


        bool check = false;

        // Bounce horizontally
        if (point.x < -halfWidth + offset)
        {
            check = true;
        }
        else if (point.x > halfWidth - offset)
        {
            check = true;
        }

        // Bounce Vertically
        if (point.z < -halfHeight + offset)
        {
            check = true; 
        }
        else if (point.z > halfHeight - offset)
        {
            check = true;
        }

        if (check)
        {
            //Debug.Log("Out of Bounds, should move to Center: \nMagnitude of Size: " + floorBounds.size.magnitude );
            //Debug.Log("point position: " + point);
        }

        return check;
    }


    public Vector3 getVehiclePosition()
    {
        return vehiclePosition;
    }


    public float getRadius()
    {
        return myRadius;
    }


    protected Vector3 AvoidAllObstacles(List<Obstacle> obsList)
    {
        Vector3 totalObsAvoid = Vector3.zero;

        foreach(Obstacle obs in obsList)
        {
            totalObsAvoid += ObstacleAvoidance(obs);
        }

        return totalObsAvoid;
    }


    protected Vector3 ObstacleAvoidance(Obstacle ob)
    {
        // Overload shortcut for convenience
        return ObstacleAvoidance(ob.obsticlePosition, ob.Radius);
    }

    protected Vector3 ObstacleAvoidance(Vector3 obsPos, float obsRadius)
    {
        // I'm going with this working on a single obstacle, should be easier

        // Need to get a vector from the vehicle to the obstacle
        Vector3 toObstacle = obsPos - vehiclePosition;

        // Check if the thing's in front 
        float forwardCheck = Vector3.Dot(forward, toObstacle);
        if(forwardCheck < 0)
        {
            // This is behind, return a zero vector
            return Vector3.zero;
        }

        // Then see if it's close enough to left or right
        float rightSideOfObs = Vector3.Dot(right, toObstacle);
        if(Mathf.Abs(rightSideOfObs) > obsRadius + myRadius)
        {
            // In this case, you can't have a collision here so you can return 0
            return Vector3.zero;
        }

        // See if the thing is close enough
        if(forwardCheck > obstacleViewDistance)
        {
            // Too far away to care, return 0
            return Vector3.zero;
        }

        // If everything above is passed, then avoid it 
        float weight = obstacleViewDistance / Mathf.Max(forwardCheck, 0.001f);

        // Determine what direction to go to avoid the obstacle
        Vector3 desiredVelocity;
        if(rightSideOfObs > 0)
        {
            // Steer left then
            desiredVelocity = right * -maxSpeed;
        }
        else
        {
            // Steer right then
            desiredVelocity = right * maxSpeed;
        }

        Vector3 steeringForce = (desiredVelocity - velocity) * weight;

        return steeringForce;
    }


    /**
    private void OnDrawGizmosSelected()
    {
        if (isMouseDown)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, mouseForce);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + velocity);
    }
    */

    
    // Draw Gizmos for Project 3
    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(transform.position, transform.position + velocity);

        // Add a DrawWireSphere for position with radius


        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(vehiclePosition, myRadius ) ;
        // (myBounds.size.x / 1.5f)
        Debug.Log("Radius size: " + myRadius);


        if (right != null)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawLine(transform.position, right);
        }

        var center = velocity.normalized;
        center *= wanderDistance;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(center + vehiclePosition, wanderRadius);

        if (floorTransform != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(floorTransform.position, floorBounds.size);

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(floorBounds.center, new Vector3(floorBounds.size.x / 2, 0, 0));
            Gizmos.color = Color.red;
            Gizmos.DrawLine(floorBounds.center, new Vector3(0, 0, floorBounds.size.z / 2));
        }

    }
    
}
