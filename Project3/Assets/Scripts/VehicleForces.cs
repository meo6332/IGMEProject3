using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleForces : MonoBehaviour
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


    // Start is called before the first frame update
    void Start()
    {
        vehiclePosition = transform.position;

        cameraObject = Camera.main;

        totalCamHeight = cameraObject.orthographicSize * 2f;

        totalCamWidth = totalCamHeight * cameraObject.aspect;

        direction = Vector3.right;

        isGravity = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isMouseDown)
        {
            // Get the mouse's position in world space values
            mouseForce = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            mouseForce.z = 0;
            ApplyForce(mouseForce - transform.position);
        }

        if (isGravity)
        {
            ApplyGravity(gravityForce);
        }


        if (isFriction)
        {
            ApplyFriction(frictionCoeff);
        }

        velocity = velocity + (acceleration * Time.deltaTime);

        vehiclePosition = vehiclePosition + (velocity * Time.deltaTime);

        direction = velocity.normalized;

        Bounce();

        transform.position = vehiclePosition;

        transform.rotation = Quaternion.LookRotation(Vector3.forward, velocity);


        acceleration = Vector3.zero;
    }


    // Add a function to set up friction and gravity forces
    void SetUpForces(Vector3 gravityVector, float newCoefficient)
    {
        gravityForce = gravityVector;
        frictionCoeff = newCoefficient;
    }




    void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }

    void ApplyFriction(float coeff)
    {
        Vector3 friction = velocity * -1;
        friction.Normalize();
        friction = friction * coeff;
        acceleration += friction;
    }

    void ApplyGravity(Vector3 force)
    {
        acceleration += force;
    }


    void Bounce()
    {
        float offset = 0.2f;
        float halfWidth = totalCamWidth / 2;
        float halfHeight = totalCamHeight / 2;

        // Wrap horizontally
        if (vehiclePosition.x < -halfWidth)
        {
            // Out of bounds on left, wrap to the right
            vehiclePosition.x = -halfWidth + offset;
            velocity.x *= -1;
           
        }
        else if (vehiclePosition.x > halfWidth)
        {
            // Out of bounds on right, wrap to the left
            vehiclePosition.x = halfWidth - offset;
            velocity.x *= -1;
        }

        // Wrap Vertically
        if (vehiclePosition.y < -halfHeight)
        {
            // Out of bounds on bottom, wrap to the top
            vehiclePosition.y = -halfHeight + offset;
            velocity.y *= -1;
        }
        else if (vehiclePosition.y > halfHeight)
        {
            // Out of bounds on top, wrap to the bottom
            vehiclePosition.y = halfHeight - offset;
            velocity.y *= -1;
        }
    }


    public void switchFriction()
    {
        isFriction = !isFriction;
    }


    public void OnClick(InputValue value)
    {
        // Value.isPressed returns true whenever the mouse button is pressed
        Debug.Log("On Click was Called");

        isMouseDown = value.isPressed;
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
    }
}
