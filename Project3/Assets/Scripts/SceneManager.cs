using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{

    [SerializeField]
    GameObject smallObject;

    [SerializeField]
    GameObject mediumObject;

    [SerializeField]
    GameObject largeObject;

    private List<GameObject> monsterList = new List<GameObject>();

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

        // Now start generating the random places to put the monsters

        float offset = 0.4f;
        float halfWidth = totalCamWidth / 2;
        float halfHeight = totalCamHeight / 2;

        Vector3 monsterPosition;

        // Create the small monster first
        float randomX = Random.Range(-halfWidth + offset, halfWidth - offset);
        float randomY = Random.Range(-halfHeight + offset, halfHeight - offset);
        monsterPosition = new Vector3(randomX, randomY, 0.0f);
        GameObject small = Instantiate(smallObject, monsterPosition, Quaternion.identity);
        small.GetComponent<VehicleForces>().mass = 1.0f;
        monsterList.Add(small);

        // Create the medium monster next
        randomX = Random.Range(-halfWidth + offset, halfWidth - offset);
        randomY = Random.Range(-halfHeight + offset, halfHeight - offset);
        monsterPosition = new Vector3(randomX, randomY, 0.0f);
        GameObject medium = Instantiate(mediumObject, monsterPosition, Quaternion.identity);
        medium.GetComponent<VehicleForces>().mass = 5.0f;
        monsterList.Add(medium);

        // Finally, create the large monster last
        randomX = Random.Range(-halfWidth + offset, halfWidth - offset);
        randomY = Random.Range(-halfHeight + offset, halfHeight - offset);
        monsterPosition = new Vector3(randomX, randomY);
        GameObject large = Instantiate(largeObject, monsterPosition, Quaternion.identity);
        large.GetComponent<VehicleForces>().mass = 20.0f;
        monsterList.Add(large);

        frictionState = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void swapFriction()
    {
        // Change the isFriction values of all the monsters
        foreach(GameObject monster in monsterList)
        {
            VehicleForces current = monster.GetComponent<VehicleForces>();

            current.switchFriction();
        }

        // Update the text in the UI
        if (frictionState)
        {
            frictionText.text = "Swap Friction\nFriction: Off";
            frictionState = false;
        }
        else
        {
            frictionText.text = "Swap Friction\nFriction: On";
            frictionState = true;
        }
    }
}
