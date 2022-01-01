using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSeeking : MonoBehaviour
{
    [SerializeField]
    bool isMouseDown = false;

    [SerializeField]
    Vector3 mouseForce;

    [SerializeField]
    Vector3 gravityForce;

    [SerializeField]
    public float mass;

    [SerializeField]
    float frictionCoeff;

    [SerializeField]
    bool isFriction;

    [SerializeField]
    bool isGravity;

    // The below values all relate to the movement of the vehicle
    [SerializeField]
    Vector3 direction = Vector3.right;

    [SerializeField]
    Vector3 vehiclePosition = Vector3.zero;

    [SerializeField]
    Vector3 velocity = Vector3.zero;

    [SerializeField]
    Vector3 acceleration = Vector3.zero;

    [SerializeField]
    float decelerationValue = 0.2f;

    [SerializeField]
    float turnSpeed = 1f;

    [SerializeField]
    float minSpeed = 0.01f;

    [SerializeField]
    Vector2 playerInput;

    // Set up the variables needed for wrapping
    [SerializeField]
    Camera cameraObject;

    [SerializeField]
    float totalCamHeight;

    [SerializeField]
    float totalCamWidth;

    [SerializeField]
    float maxSpeed;





    Vector3 seekForce;

    Vector3 mousePos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        seekForce = Seek(mousePos);
        ApplyForce(seekForce);
    }


    public Vector3 Seek(Vector3 targetPos)
    {
        // Steering forces are intended to get an object to face towards an object
        Vector3 desiredVelocity = targetPos - vehiclePosition;

        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        return desiredVelocity - velocity;
    }

    public Vector3 Seek(VehicleSeeking targetVehicle)
    {
        // Exists to make it easier to Seek a vehicle without grabbing it's
        // position directly
        return Seek(targetVehicle.transform.position);
    }

    void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }


    private void OnDrawGizmosSelected()
    {
        if (isMouseDown)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, mouseForce);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + velocity);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + seekForce);
    }
}
