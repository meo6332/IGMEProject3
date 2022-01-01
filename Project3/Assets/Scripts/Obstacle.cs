using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{


    private float radius = 1;

    public float Radius => radius;

    public Vector3 obsticlePosition;

    public MeshRenderer mesh;

    // Start is called before the first frame update
    void Start()
    {

        mesh = this.GetComponent<MeshRenderer>();
        obsticlePosition = transform.position;
        radius = mesh.bounds.extents.x;

        Debug.Log("Obstacle Radius: " + radius);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // Add a DrawWireSphere for position with radius


        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, radius);

    }
}
