using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentManager : MonoBehaviour
{

    [SerializeField]
    GameObject fleer;

    [SerializeField]
    GameObject seeker;

    private List<GameObject> monsterList = new List<GameObject>();

    private CollisionDetection detection;

    [SerializeField]
    Camera cameraObject;

    [SerializeField]
    float totalCamHeight;

    [SerializeField]
    float totalCamWidth;

    [SerializeField]
    Text frictionText;

    private bool frictionState;

    // Start is called before the first frame update
    void Start()
    {
        // Get the camera info
        cameraObject = Camera.main;

        totalCamHeight = cameraObject.orthographicSize * 2f;

        totalCamWidth = totalCamHeight * cameraObject.aspect;

        // Get the collisionDetection script
        detection = GetComponent<CollisionDetection>();

        // Now start generating the random places to put the monsters

        float offset = 0.4f;
        float halfWidth = totalCamWidth / 2;
        float halfHeight = totalCamHeight / 2;

        Vector3 monsterPosition;

        

        // Create the fleer first
        float randomX = Random.Range(-halfWidth + offset, halfWidth - offset);
        float randomY = Random.Range(-halfHeight + offset, halfHeight - offset);
        monsterPosition = new Vector3(randomX, randomY);
        GameObject large = Instantiate(fleer, monsterPosition, Quaternion.identity);
        large.GetComponent<VehicleAbstract>().SetUpPosition(monsterPosition);
        //large.GetComponent<VehicleForces>().mass = 1.0f;
        monsterList.Add(large);

        // Create the seeker second
        randomX = Random.Range(-halfWidth + offset, halfWidth - offset);
        randomY = Random.Range(-halfHeight + offset, halfHeight - offset);
        monsterPosition = new Vector3(randomX, randomY);
        GameObject small = Instantiate(seeker, monsterPosition, Quaternion.identity);
        small.GetComponent<VehicleAbstract>().SetUpPosition(monsterPosition);
        //small.GetComponent<VehicleForces>().mass = 1.0f;
        monsterList.Add(small);


        frictionState = false;
    }

    // Update is called once per frame
    void Update()
    {
        bool check = false;

        // If they are colliding, move the fleer to a random location on the screen
        if (check)
        {
            float offset = 0.4f;
            float halfWidth = totalCamWidth / 2;
            float halfHeight = totalCamHeight / 2;

            float randomX = Random.Range(-halfWidth + offset, halfWidth - offset);
            float randomY = Random.Range(-halfHeight + offset, halfHeight - offset);

            monsterList[0].GetComponent<VehicleAbstract>().SetNewPosition(new Vector3(randomX, randomY));

        }
    }
}
