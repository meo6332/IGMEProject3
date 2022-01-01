using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    // Variables related to the projectile's movement
    [SerializeField]
    Vector3 bulletPosition = Vector3.zero;

    [SerializeField]
    Vector3 direction;

    [SerializeField]
    Vector3 velocity = Vector3.zero;

    [SerializeField]
    float bulletSpeed = 2.0f;

    public bool fired = false;

    private GameManager manager;

    // Info about the floor for bounds checking
    Bounds floorBounds;

    [SerializeField]
    private float bulletRadius = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        // Find the projectile manager
        manager = FindObjectOfType<GameManager>();

        // Set the bullet's default position
        transform.position = manager.idleBulletPos;
        bulletPosition = manager.idleBulletPos;

        var objects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject o in objects)
        {
            if (o.name.Contains("floor"))
            {
                Transform worldTransform = o.transform;
                floorBounds = new Bounds(worldTransform.position, 10 * worldTransform.lossyScale);

                Debug.Log(floorBounds.size);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (fired)
        {
            // Need to move the bullet
            bulletPosition = bulletPosition + (velocity * Time.deltaTime);

            if (CheckIsPointOutBounds(bulletPosition))
            {
                // If this is the case, despawn self
                despawnSelf();
            }

            transform.position = bulletPosition;

            transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

            bool colliding = false;



            // Now do collision detection for the zombies
            foreach(Zombie v in manager.zombieList)
            {

                // Check for each zombie, and see if I'm colliding with any
                Vector3 zombiePos = v.getVehiclePosition();
                float zombieRad = v.getRadius();
                colliding = manager.CircleCollisions(bulletPosition, bulletRadius, zombiePos, zombieRad);

                
                if (colliding)
                {
                    v.despawnSelf();
                    break;
                }
            }

            if (colliding)
            {
                despawnSelf();
            }

        }
    }


    bool CheckIsPointOutBounds(Vector3 point)
    {
        // fill in the logic here
        float baseOffset = 0.0f;
        float offset = 0.2f;
        float halfWidth = floorBounds.size.x / 2;
        float halfHeight = floorBounds.size.z / 2;

        // Debug.Log("point position: " + point);


        bool check = false;

        // Bounce horizontally
        if (point.x < -halfWidth)
        {
            check = true;
        }
        else if (point.x > halfWidth)
        {
            check = true;
        }

        // Bounce Vertically
        if (point.z < -halfHeight)
        {
            check = true;
        }
        else if (point.z > halfHeight)
        {
            check = true;
        }

        if (check)
        {
            Debug.Log("Bullet Out of Bounds, should despawn: \nMagnitude of Size: " + floorBounds.size.magnitude);
            Debug.Log("point position: " + point);
        }

        return check;
    }


    /// <summary>
    /// This function handles whenever the bullet is fired, setting it's new direction
    /// and velocity, as well as setting flags to allow it to move.
    /// </summary>
    /// <param name="newDirection">
    /// The new direction that the bullet will be traveling in.
    /// </param>
    /// <param name="newPosition">
    /// The updated initial position of the bullet it will start moving from
    /// </param>
    public void fireBullet(Vector3 newDirection, Vector3 newPosition)
    {
        // First update the bullet to be positioned at the ship
        bulletPosition = newPosition;

        this.transform.position = bulletPosition;

        // Make the direction for the bullet and determine the constant velocity
        direction = newDirection;

        velocity = direction * bulletSpeed;

        // Set the firing flag to true
        fired = true;

        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
    }

    /// <summary>
    /// This function handles despawning the bullet, stopping it from moving,
    /// moving it offscreen, and then returning it to the ProjectileManager's queue of bullets.
    /// </summary>
    public void despawnSelf()
    {
        // This function will despawn the bullet and return it to the manager's queue

        fired = false;
        // Put the bullet offscreen so that issues don't arrise from it colliding with asteroids
        transform.position = manager.idleBulletPos;
        bulletPosition = manager.idleBulletPos;
        manager.ReturnBullet(this.gameObject);

    }


}
