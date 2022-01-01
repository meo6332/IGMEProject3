using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool AABBCollision(GameObject first, GameObject second)
    {
        // This will contain the final value to be returned
        bool result = false;

        Bounds firstBounds = first.GetComponent<SpriteInfo>().spriteBounds;
        Bounds secondBounds = second.GetComponent<SpriteInfo>().spriteBounds;

        // Get the values for the first object
        float minXFirst = firstBounds.min.x;
        float maxXFirst = firstBounds.max.x;
        float minYFirst = firstBounds.min.y;
        float maxYFirst = firstBounds.max.y;

        // Get the values for the second object
        float minXSecond = secondBounds.min.x;
        float maxXSecond = secondBounds.max.x;
        float minYSecond = secondBounds.min.y;
        float maxYSecond = secondBounds.max.y;

        // Checking the 4 conditions in a nested if
        // Condition 1
        if (minXSecond < maxXFirst)
        {
            // Condition 2
            if(maxXSecond > minXFirst)
            {
                // Condition 3
                if(maxYSecond > minYFirst)
                {
                    // Condition 4
                    if(minYSecond < maxYFirst)
                    {
                        // If all of the above are true, there's a collision
                        result = true;

                        Debug.Log("Colliding with AABB");
                    }
                }
            }
        }

        return result;
    }


    public bool CircleCollision(Bounds firstBounds, Bounds secondBounds)
    {
        // This will contain the final value to be returned
        bool result = false;

        float offset = 0.4f;

        // Get the radius of the two objects
        float firstRadius = firstBounds.extents.magnitude - offset;
        float secondRadius = secondBounds.extents.magnitude - offset;

        Vector3 firstCenter = firstBounds.center;
        Vector3 secondCenter = secondBounds.center;

        // Calculate the distance
        float distance = Mathf.Sqrt(Mathf.Pow(firstCenter.x - secondCenter.x, 2.0f) 
            + Mathf.Pow(firstCenter.y - secondCenter.y, 2.0f));

        if(distance <= (firstRadius + secondRadius))
        {
            // If true, you have a collision
            result = true;

            Debug.Log("Circle Colliding occuring");
        }

        return result;
    }
}
