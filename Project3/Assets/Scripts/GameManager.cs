using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public List<VehicleAbstract> humansList;

    public Queue<VehicleAbstract> zombieList;

    [SerializeField]
    private string obstacleName;

    public List<Obstacle> obstaclesList;

    [SerializeField]
    private GameObject zombiePrefab;

    // The below fields are for the bullet pool behavior
    [SerializeField]
    GameObject bulletPrefab;

    [SerializeField]
    Queue<GameObject> bulletPool = new Queue<GameObject>();

    [SerializeField]
    int poolSize = 10;

    [SerializeField]
    public Vector3 idleBulletPos = new Vector3(0.0f, -1000.0f, 1000.0f);

    [SerializeField]
    public Vector3 idleZombiePos = new Vector3(0.0f, -10.0f, 0.0f);

    // Get the info about the floor needed to find the bounds 
    [SerializeField]
    private GameObject floor;

    private Bounds floorBounds;

    [SerializeField]
    private float zombieSpawnOffset = 1.0f;

    private float initialSpawnDelay;

    [SerializeField]
    private float zombieSpawnDelay = 10.0f;

    // Info needed for the UI
    [SerializeField]
    private Text timerText;

    [SerializeField]
    private Text humansAliveText;

    private float timer = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        humansList = new List<VehicleAbstract>();
        zombieList = new Queue<VehicleAbstract>();
        obstaclesList = new List<Obstacle>();

        Transform worldTransform = floor.transform;
        floorBounds = new Bounds(worldTransform.position, 10 * worldTransform.lossyScale);

        initialSpawnDelay = zombieSpawnDelay;

        PopulateHumans();
        PopulateZombies();

        // I may need to make a populate obstacles, but unneeded for now given they're not instantiated
        PopulateObstacles();

        PopulateBullets();
    }

    // Update is called once per frame
    void Update()
    {
        // Before anything else, I need to see if a zombie should get spawned
        SpawnZombie();

        var humansToRemove = new List<VehicleAbstract>();
        var newZombiePos = new List<Vector3>();

        // Need to do collision detection between every human and zombie 
        foreach(VehicleAbstract h in humansList)
        {
            foreach(Zombie z in zombieList)
            {
                // Check if the zombie is active first 
                if (!z.active)
                {
                    continue;
                }

                // Do the check if the human is coliding with a zombie or not
                bool result = CircleCollisions(h, z);

                if (result)
                {
                    // Then the two are colliding, add h to the list of humans to remove
                    // And get it's current position
                    if (!humansToRemove.Contains(h)) 
                    {
                        // Only add the human and do the below calcs if the human isn't already there
                        Vector3 newPos = h.getVehiclePosition();
                        newPos.y = 1.0f;

                        humansToRemove.Add(h);
                        newZombiePos.Add(newPos);

                    } 

                }
            }  
        }

        // Did all the collision checks for humans, so now see what ones need to become zombies
        if (humansToRemove.Count > 0)
        {
            for (int i = 0; i < humansToRemove.Count; i++)
            {
                VehicleAbstract current = humansToRemove[i];
                Vector3 newPos = newZombiePos[i];
                

                // Remove the current human and spawn a new zombie in it's place
                humansList.Remove(current);
                current.DestroySelf();
               

                GameObject newZombie = Instantiate(zombiePrefab, newPos, Quaternion.identity);
                Zombie script = newZombie.GetComponent<Zombie>();
                script.active = true;
                zombieList.Enqueue(script);
            }
        }

        timer += Time.deltaTime;
        UpdateUI();

    }


    public void PopulateHumans()
    {
        var objects = FindObjectsOfType<GameObject>();

        foreach (GameObject o in objects)
        {
            if (o.name.Contains("human"))
            {
                if (!humansList.Contains(o.GetComponent<Human>()))
                {
                    
                    humansList.Add(o.GetComponent<Human>());
                    
                }
            }
        }
    }

    public void PopulateZombies()
    {
        var objects = FindObjectsOfType<GameObject>();

        foreach (GameObject o in objects)
        {
            if (o.name.Contains("zombie"))
            {
                if (!zombieList.Contains(o.GetComponent<Zombie>()))
                {
                    zombieList.Enqueue(o.GetComponent<Zombie>());
                }
            }
        }
    }

    public void PopulateObstacles()
    {
        var objects = FindObjectsOfType<GameObject>();

        foreach (GameObject o in objects)
        {
            if (o.name.Contains(obstacleName))
            {
                if (!obstaclesList.Contains(o.GetComponent<Obstacle>()))
                {
                    obstaclesList.Add(o.GetComponent<Obstacle>());
                }
            }
        }
    }

    private void PopulateBullets()
    {
        // Start by making bullets to put in the pool, and making them inactive
        for (int j = 0; j < poolSize; j++)
        {
            // Spawn the bullet off of the map
            GameObject bullet = Instantiate(bulletPrefab, idleBulletPos, Quaternion.identity);

            bulletPool.Enqueue(bullet);

            bullet.SetActive(false);
        }
    }



    private bool CircleCollisions(VehicleAbstract first, VehicleAbstract second)
    {
        float rad1 = first.getRadius();
        float rad2 = second.getRadius();
        Vector3 pos1 = first.getVehiclePosition();
        Vector3 pos2 = second.getVehiclePosition();

        return CircleCollisions(pos1, rad1, pos2, rad2);
    }

    public bool CircleCollisions(Vector3 firstCenter, float firstRadius, Vector3 secondCenter, float secondRadius)
    {
        bool result = false;

        float offSet = 0.4f;

        // Calculate the distance

        // Hold on do the distance formula calcs need to change in 3d
        // Change the y to z, as we want to ignore the y componenet?
        float distance = Mathf.Sqrt(Mathf.Pow(firstCenter.x - secondCenter.x, 2.0f)
            + Mathf.Pow(firstCenter.z - secondCenter.z, 2.0f));

        if (distance <= (firstRadius + secondRadius))
        {
            // If true, you have a collision
            result = true;

            Debug.Log("Circle Colliding occuring");
        }

        return result;
    }


    /// <summary>
    /// Retrieves a bullet from the bullet pool, if there is one.
    /// Will make the bullet active.
    /// </summary>
    /// <returns>
    /// A bullet GameObject from the bullet pool
    /// </returns>
    public GameObject GetBullet()
    {
        GameObject current;

        if (bulletPool.Count > 0)
        {
            // In this case, have a bullet to get from the pool, return that
            current = bulletPool.Dequeue();
            current.SetActive(true);
        }
        else
        {
            // Not enough bullets, make a new one
            current = Instantiate(bulletPrefab);
        }

        return current;
    }


    /// <summary>
    /// Puts a bullet back into the bulletPool and sets it to be inactive.
    /// </summary>
    /// <param name="bullet">
    /// The bullet to be returned to the bullet pool.
    /// </param>
    public void ReturnBullet(GameObject bullet)
    {
        // This will take a bullet and add it back to the pool while disabling it
        bulletPool.Enqueue(bullet);
        bullet.SetActive(false);
    }


    private void SpawnZombie()
    {
        // This will check for spawning a zombie
        if(zombieSpawnDelay < 0)
        {
            // In this case spawn a zombie
            Vector3 newPos = GenerateEdgePosition();

            // Look through the list of zombies and see if any of them are active
            Zombie toSpawn = null;
            foreach(Zombie z in zombieList)
            {
                if (!z.active)
                {
                    // If the zombie isn't active, set it to be the one to spawn
                    toSpawn = z;
                }
            }

            if(toSpawn != null)
            {
                // Then spawn this zombie
                toSpawn.spawnSelf(newPos);
            }

            zombieSpawnDelay = initialSpawnDelay;
        }
        else
        {
            zombieSpawnDelay -= Time.deltaTime;
        }

    }

    private Vector3 GenerateEdgePosition()
    {
        // This exists to generate a position along the edge of the map, where zombies get spawned
        var newPos = Vector3.zero;

        float side = Random.Range(0, 4);
        // 0-1 is right, 1-2 is up, 2-3 is left, 3-4 is down

        float zVal = 0.0f;
        float xVal = 0.0f;

        if(side < 1)
        {
            // Starting at the right
            // Generate a z position between the edge and edge-offset
            float zEdge = floorBounds.size.z / 2;
            zVal = Random.Range(zEdge - zombieSpawnOffset, zEdge);
            float xEdge = floorBounds.size.x / 2;
            xVal = Random.Range(-xEdge, xEdge);
        }
        else if (side < 2)
        {
            // Start at up
            float zEdge = floorBounds.size.z / 2;
            zVal = Random.Range(-zEdge, zEdge);
            float xEdge = floorBounds.size.x / 2;
            xVal = Random.Range(-xEdge, -xEdge + zombieSpawnOffset);
        }
        else if(side < 3)
        {
            // Start at left
            float zEdge = floorBounds.size.z / 2;
            zVal = Random.Range(-zEdge, -zEdge + zombieSpawnOffset);
            float xEdge = floorBounds.size.x / 2;
            xVal = Random.Range(-xEdge, xEdge);
        }
        else
        {
            // Start at down
            float zEdge = floorBounds.size.z / 2;
            zVal = Random.Range(-zEdge, zEdge);
            float xEdge = floorBounds.size.x / 2;
            xVal = Random.Range(xEdge - zombieSpawnOffset, xEdge);
        }

        newPos = new Vector3(xVal, 1.0f, zVal);

        return newPos;
    }

    private void UpdateUI()
    {

        string hText = "Humans Remaining: ";
        hText += humansList.Count.ToString();

        string tText = "Time Survived: ";
        int intTime = (int) timer % 60;
        tText += intTime.ToString();

        timerText.text = tText;
        humansAliveText.text = hText;
    }

}
